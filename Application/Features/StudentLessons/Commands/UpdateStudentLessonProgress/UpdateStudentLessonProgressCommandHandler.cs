using Application.Features.StudentLessons.Dtos;
using Application.Helpers;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Features.StudentLessons.Commands.UpdateStudentLessonProgress
{
    internal sealed class UpdateStudentLessonProgressCommandHandler : IRequestHandler<UpdateStudentLessonProgressCommand, OneOf<LessonProgressDto, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStudentSubscriptionRepository _studentSubscriptionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILessonViewRepository _lessonViewRepository;

        public UpdateStudentLessonProgressCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor,
            IStudentSubscriptionRepository studentSubscriptionRepository, IEnrollmentRepository enrollmentRepository,
            IModuleItemRepository moduleItemRepository, IUnitOfWork unitOfWork, ILessonViewRepository lessonViewRepository)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _studentSubscriptionRepository = studentSubscriptionRepository;
            _enrollmentRepository = enrollmentRepository;
            _moduleItemRepository = moduleItemRepository;
            _unitOfWork = unitOfWork;
            _lessonViewRepository = lessonViewRepository;
        }

        public async Task<OneOf<LessonProgressDto, Error>> Handle(UpdateStudentLessonProgressCommand request, CancellationToken cancellationToken)
        {
            var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SessionId];
            var cachedSessionKey = $"{CacheKeysConstants.SessionKey}_{sessionId}";

            var session = await _hybridCache.GetOrCreateAsync<UserSession?>(
                cachedSessionKey,
                _ => ValueTask.FromResult<UserSession?>(null),
                cancellationToken: cancellationToken);

            if (session is null)
                return UserErrors.Unauthorized;

            var isEnrolled = await _enrollmentRepository.StudentIsAlreadyEnrolledAsync(session.StudentId, request.CourseId, cancellationToken);
            if (!isEnrolled)
                return StudentCourseErrors.StudentNotEnrolledInCourse;

            var subscriptionIsActive = await _studentSubscriptionRepository.StudentSubscriptionIsActiveAsync(session.StudentId, request.CourseId, cancellationToken);
            if (!subscriptionIsActive)
                return StudentSubscriptionErrors.StudentSubscribedExpired;

            var moduleItemIsExist = await _moduleItemRepository.ModuleItemIsExistAsync(request.ItemId, request.CourseId, cancellationToken);
            if (!moduleItemIsExist)
                return ModuleItemErrors.ModuleItemNotFound;

            var normalizedLastPosition = Math.Clamp(request.LastPosition, 0, request.Duration);
            var newSegments = SegmentHelper.NormalizeIncomingSegments(request.Segments, request.Duration);

            if (newSegments.Count == 0 && normalizedLastPosition == 0)
                return new Error("InvalidSegments", "No valid segments or position provided", System.Net.HttpStatusCode.BadRequest);

            var lessonView = await _lessonViewRepository.GetLessonViewAsync(session.StudentId, request.ItemId, cancellationToken);
            if (lessonView is null)
            {
                lessonView = new LessonView
                {
                    Status = ViewStatus.InProgress,
                    WatchedSeconds = newSegments.Count > 0 ? (int)SegmentHelper.CalculateTotalWatched(newSegments) : 0,
                    LastPositionSeconds = normalizedLastPosition,
                    ModuleItemId = request.ItemId,
                    StudentId = session.StudentId,
                    ViewCount = 0,
                    Device = "Mobile",
                };
                await _lessonViewRepository.CreateLessonViewAsync(lessonView, cancellationToken);
            }
            else
            {
                lessonView.LastPositionSeconds = normalizedLastPosition;
                lessonView.LastWatchedAt = DateTime.UtcNow;
            }

            var existingSegments = lessonView.VideoSegmants
                .Where(x => x.EndSecond > x.StartSecond)
                .Select(x => ((double)x.StartSecond, (double)x.EndSecond))
                .ToList();

            var mergedSegments = SegmentHelper.MergeRanges(existingSegments.Concat(newSegments));
            var totalWatched = SegmentHelper.CalculateTotalWatched(mergedSegments);
            var watchedRatio = Math.Min(1.0, totalWatched / request.Duration);
            var completionPercentage = SegmentHelper.RoundToTwo(watchedRatio * 100);
            var isCompleted = watchedRatio >= SegmentHelper.CompletionThreshold;

            var previousWatched = lessonView.WatchedSeconds;
            var finalWatchedSeconds = Math.Max(previousWatched, (int)Math.Round(totalWatched));

            var crossedViewThreshold = previousWatched < SegmentHelper.ViewCountThreshold
                && totalWatched >= SegmentHelper.ViewCountThreshold;

            var nextViewsCount = lessonView.ViewCount + (crossedViewThreshold ? 1 : 0);

            lessonView.VideoSegmants.Clear();
            foreach (var segment in mergedSegments)
            {
                lessonView.VideoSegmants.Add(new LessonVideoSegmant
                {
                    StartSecond = (int)segment.Start,
                    EndSecond = (int)segment.End
                });
            }

            lessonView.WatchedSeconds = finalWatchedSeconds;
            lessonView.Status = isCompleted ? ViewStatus.Completed : ViewStatus.InProgress;
            lessonView.ViewCount = nextViewsCount;
            lessonView.LastWatchedAt = DateTime.UtcNow;

            await _unitOfWork.SaveAsync(cancellationToken);
            return new LessonProgressDto
            {
                ItemId = request.ItemId,
                CourseId = request.CourseId,
                VideoId = request.VideoId,
                WatchedSeconds = lessonView.WatchedSeconds,
                DurationSeconds = request.Duration,
                CompletionPercentage = completionPercentage,
                IsCompleted = isCompleted,
                LastPositionSeconds = lessonView.LastPositionSeconds,
                ViewsCount = lessonView.ViewCount,
                FirstViewedAt = lessonView.CreatedAt,
                LastViewedAt = lessonView.LastWatchedAt
            };
        }
    }
}