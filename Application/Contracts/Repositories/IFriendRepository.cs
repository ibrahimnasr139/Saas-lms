using Application.Features.Friends.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IFriendRepository
    {
        Task CreateRequestAsync(Friend friend, CancellationToken cancellationToken);
        Task<List<FriendsDto>> GetFriendsAsync(int studentId, CancellationToken cancellationToken);
        Task<RequestsDto> GetRequestsAsync(int studentId, CancellationToken cancellationToken);
        Task<bool> RequestAlreadySentAsync(int studentSenderId, int studentRecevierId, CancellationToken cancellationToken);
        Task<Friend?> GetFriendRequestPendingAsync(int requestId, CancellationToken cancellationToken);
        Task UpdateRequestStatusAsync(Friend request, CancellationToken cancellationToken);
    }
}