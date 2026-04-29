using Application.Contracts.Repositories;
using Application.Features.Discussions.Commands.CreateDiscussionThreadRead;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Discussions.Commands.MarkThreadAsRead
{
    internal sealed class CreateDiscussionThreadReadCommandHandler : IRequestHandler<CreateDiscussionThreadReadCommand, OneOf<bool, Error>>
    {
        private readonly IDiscussionRepository _discussionRepository;
        private readonly ICurrentUserId _currentUserId;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDiscussionThreadReadCommandHandler(IDiscussionRepository discussionRepository, ICurrentUserId currentUserId,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _discussionRepository = discussionRepository;
            _currentUserId = currentUserId;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<OneOf<bool, Error>> Handle(CreateDiscussionThreadReadCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var thread = await _discussionRepository.GetThreadTenantAsync(request.ThreadId, subDomain!, cancellationToken);
            if (thread is null)
                return DiscussionErrors.DiscussionThreadNotFound;

            var userId = _currentUserId.GetUserId();
            var newDiscussionThreadRead = new DicussionThreadRead
            {
                DicussionId = request.ThreadId,
                UserId = userId,
                TenantId = thread.TenantId,
            };
            await _discussionRepository.CreateDiscussionThreadReadAsync(newDiscussionThreadRead, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return true;
        }
    }
}
