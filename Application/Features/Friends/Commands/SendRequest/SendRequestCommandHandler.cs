using Application.Features.Friends.Dtos;
using Application.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Friends.Commands.SendRequest
{
    internal sealed class SendRequestCommandHandler : IRequestHandler<SendRequestCommand, OneOf<FriendResponse, Error>>
    {
        private readonly HybridCache _hybridCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFriendRepository _friendRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public SendRequestCommandHandler(HybridCache hybridCache, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork,
            IFriendRepository friendRepository, IStudentRepository studentRepository, IEmailSender emailSender)
        {
            _hybridCache = hybridCache;
            _httpContextAccessor = httpContextAccessor;
            _friendRepository = friendRepository;
            _studentRepository = studentRepository;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<FriendResponse, Error>> Handle(SendRequestCommand request, CancellationToken cancellationToken)
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

            var studentSenderName = await _studentRepository.GetStuentNameByIdAsync(session.StudentId, cancellationToken);
            var studentReceiver = await _studentRepository.GetStudentByInviteCodeAsync(request.InviteCode, cancellationToken);
            if (session.StudentId == studentReceiver!.Id)
                return FriendErrors.CannotRequestYourself;

            if (await _friendRepository.RequestAlreadySentAsync(session.StudentId, studentReceiver.Id, cancellationToken))
                return FriendErrors.RequestAlreadyExists;

            var newRequestFriend = new Friend
            {
                Student1Id = Math.Min(session.StudentId, studentReceiver.Id),
                Student2Id = Math.Max(session.StudentId, studentReceiver.Id),
                ActionStudentId = session.StudentId
            };
            await _friendRepository.CreateRequestAsync(newRequestFriend, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                EmailConstants.RequestFriendTemplate,
                new Dictionary<string, string>
                {
                    { "{{UserName}}", studentReceiver.User.FirstName+" "+studentReceiver.User.LastName },
                    { "{{RequesterName}}", studentSenderName },
                    { "{{VIEW_REQUEST_URL}}", $"{EmailConstants.RequestFriendUrl}" },
                });

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(studentReceiver.User.Email!, EmailConstants.RequestFriendSubject, emailBody));
            return new FriendResponse { Message = $"{MessagesConstants.RequestFriendSuccessfully} {studentReceiver.User.Email!}" };
        }
    }
}