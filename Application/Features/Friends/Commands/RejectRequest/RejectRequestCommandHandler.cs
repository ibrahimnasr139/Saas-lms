using Application.Features.Friends.Dtos;
using Application.Helpers;
using Domain.Enums;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Friends.Commands.RejectRequest
{
    internal sealed class RejectRequestCommandHandler : IRequestHandler<RejectRequestCommand, OneOf<FriendResponse, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFriendRepository _friendRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public RejectRequestCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork,
            IFriendRepository friendRepository, IStudentRepository studentRepository, IEmailSender emailSender)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _friendRepository = friendRepository;
            _studentRepository = studentRepository;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<FriendResponse, Error>> Handle(RejectRequestCommand request, CancellationToken cancellationToken)
        {
            var sessionId = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SessionId];
            var cachedSessionKey = $"{CacheKeysConstants.SessionKey}_{sessionId}";
            var session = await _hybridCache.GetOrCreateAsync<UserSession?>(
                cachedSessionKey,
                _ => ValueTask.FromResult<UserSession?>(null),
                cancellationToken: cancellationToken
            );
            if (session is null)
                return UserErrors.Unauthorized;

            var friendRequest = await _friendRepository.GetFriendRequestPendingAsync(request.RequestId, cancellationToken);
            if (friendRequest is null)
                return FriendErrors.RequestNotFound;

            var senderStudentId = friendRequest.ActionStudentId;
            var senderStudent = await _studentRepository.GetStudentByIdAsync(senderStudentId, cancellationToken);
            var studentAcceptedName = await _studentRepository.GetStuentNameByIdAsync(session.StudentId, cancellationToken);

            friendRequest.ActionStudentId = session.StudentId;
            friendRequest.Status = FriendStatus.Declined;
            await _friendRepository.UpdateRequestStatusAsync(friendRequest, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                EmailConstants.RejectRequestTemplate,
                new Dictionary<string, string>
                {
                    { "{{UserName}}", senderStudent.User.FirstName+" "+senderStudent.User.LastName },
                    { "{{FriendName}}", studentAcceptedName },
                    { "{{OPEN_APP_URL}}", EmailConstants.RequestFriendUrl },
                });
            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(senderStudent.User.Email!, EmailConstants.RejectRequestSubject, emailBody));
            return new FriendResponse { Message = MessagesConstants.RejectRequestSuccessfully };
        }
    }
}