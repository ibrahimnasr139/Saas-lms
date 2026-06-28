using Application.Features.StudentLessons.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Discussions.Commands.CreateDiscussionReply
{
    internal sealed class CreateDiscussionReplyCommandHandler : IRequestHandler<CreateDiscussionReplyCommand, OneOf<StudentLessonResponse, Error>>
    {
        private readonly ICurrentUserId _currentUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IDiscussionRepository _discussionRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateDiscussionReplyCommandHandler(ICurrentUserId currentUserId, IHttpContextAccessor httpContextAccessor,
            ITenantRepository tenantRepository, ISubscriptionRepository subscriptionRepository,
            IDiscussionRepository discussionRepository, IModuleItemRepository moduleItemRepository, IUnitOfWork unitOfWork)
        {
            _currentUserId = currentUserId;
            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _subscriptionRepository = subscriptionRepository;
            _discussionRepository = discussionRepository;
            _moduleItemRepository = moduleItemRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<StudentLessonResponse, Error>> Handle(CreateDiscussionReplyCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserId.GetUserId();
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var tenantId = await _tenantRepository.GetTenantIdAsync(subDomain!, cancellationToken);

            var isSubscribed = await _subscriptionRepository.HasActiveSubscriptionByTenantDomain(subDomain!, cancellationToken);
            if (!isSubscribed)
                return TenantErrors.NotSubscribed;

            var moduleItemIsExist = await _moduleItemRepository.ModuleItemIsExistAsync(request.ItemId, request.CourseId, cancellationToken);
            if (!moduleItemIsExist)
                return ModuleItemErrors.ModuleItemNotFound;

            var discussion = await _discussionRepository.GetDicussionThreadAsync(request.ThreadId, cancellationToken);
            if (discussion is null)
                return DiscussionErrors.DiscussionThreadNotFound;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                discussion.RepliesCount += 1;

                var newDiscussionReply = new DicussionThreadReply
                {
                    Body = request.Content,
                    AuthorId = userId,
                    DicussionId = request.ThreadId,
                    TenantId = tenantId,
                };
                await _discussionRepository.CreateDiscussionThreadReplyAsync(newDiscussionReply, cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);
                return new StudentLessonResponse { Messsage = MessagesConstants.DiscussionReplyCreated };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}