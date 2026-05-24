using Application.Features.Discussions.Dtos;
using Application.Features.StudentLessons.Dtos;
using Domain.Enums;

namespace Application.Contracts.Repositories
{
    public interface IDiscussionRepository
    {
        Task CreateDiscussionThreadAsync(DicussionThread dicussionThread, CancellationToken cancellationToken);
        Task CreateDiscussionThreadReplyAsync(DicussionThreadReply reply, CancellationToken cancellationToken);
        Task CreateDiscussionThreadReadAsync(DicussionThreadRead dicussionThreadRead, CancellationToken cancellationToken);
        Task<AllDiscussionsDto> GetAllDiscussionsAsync(string subDomain, string currentUser, string? Q, int? CourseId, ModuleItemType? Type, int? Cursor, int? Limit, int? ModuleId, int? ItemId, CancellationToken cancellationToken);
        Task<List<DiscussionReplyDto>> GetDiscussionReplyAsync(int threadId, CancellationToken cancellationToken);
        Task<DicussionThread?> GetThreadTenantAsync(int threadId, string subDomain, CancellationToken cancellationToken);
        Task<DiscussionStatisticsDto> GetDiscussionStatisticsAsync(string subDomain, CancellationToken cancellationToken);
        Task<bool> DeleteDiscussionThreadAsync(int threadId, string subDomain, CancellationToken cancellationToken);
        Task<bool> DeleteDiscussionThreadReplyAsync(int threadId, int replyId, string subDomain, CancellationToken cancellationToken);
        Task<bool> UpdateDiscussionReplyAsync(int threadId, int replyId, string body, string subDomain, CancellationToken cancellationToken);
        Task<List<StudentDiscussionDto>> GetStudentDiscussionAsync(int itemId, int courseId, CancellationToken cancellationToken);
        Task<DicussionThread?> GetDicussionThreadAsync(int discussionId, CancellationToken cancellationToken);
        Task<bool> IsDiscussionOwnerAsync(int discussionId, string userId, CancellationToken cancellationToken);
        Task DeleteDiscussionThreadAsync(DicussionThread dicussionThread, CancellationToken cancellationToken);
        Task<bool> IsDiscussionReplyOwnerAsync(int replyId, int discussionId, string userId, CancellationToken cancellationToken);
        Task<DicussionThreadReply?> GetDicussionThreadReplyAsync(int replyId, CancellationToken cancellationToken);
        Task DeleteDiscussionThreadReplyAsync(DicussionThreadReply reply, CancellationToken cancellationToken);
    }
}