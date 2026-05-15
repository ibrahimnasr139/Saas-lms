using Application.Features.Friends.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class FriendRepository : IFriendRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FriendRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task CreateRequestAsync(Friend friend, CancellationToken cancellationToken)
        {
            await _context.Friends.AddAsync(friend, cancellationToken);
        }
        public async Task<List<FriendsDto>> GetFriendsAsync(int studentId, CancellationToken cancellationToken)
        {
            return await _context.Friends
                .AsNoTracking()
                .Where(f => f.Status == FriendStatus.Accepted && (f.Student1Id == studentId || f.Student2Id == studentId))
                .Select(f => new FriendsDto
                {
                    Id = f.Student1Id == studentId
                        ? f.Student2Id : f.Student1Id,

                    Name = f.Student1Id == studentId
                        ? f.Student2.User.FirstName + " " + f.Student2.User.LastName
                        : f.Student1.User.FirstName + " " + f.Student1.User.LastName,

                    ProfilePicture = f.Student1Id == studentId
                        ? f.Student2.User.ProfilePicture
                        : f.Student1.User.ProfilePicture,

                    Grade = f.Student1Id == studentId
                        ? f.Student2.Grade
                        : f.Student1.Grade,

                    XP = f.Student1Id == studentId
                        ? f.Student2.XP
                        : f.Student1.XP,

                    Level = f.Student1Id == studentId
                        ? f.Student2.Level
                        : f.Student1.Level,

                    CurrentStreak = f.Student1Id == studentId
                        ? (f.Student2.StudentStreak != null ? f.Student2.StudentStreak.CurrentStreak : 0)
                        : (f.Student1.StudentStreak != null ? f.Student1.StudentStreak.CurrentStreak : 0)
                }).ToListAsync(cancellationToken);
        }
        public async Task<RequestsDto> GetRequestsAsync(int studentId, CancellationToken cancellationToken)
        {
            var incomingRequests = await _context.Friends
                .AsNoTracking()
                .Where(f => f.Status == FriendStatus.Pending
                    && f.ActionStudentId != studentId
                    && (f.Student1Id == studentId || f.Student2Id == studentId))
                .Select(f => new FriendRequestDto
                {
                    Id = f.Student1Id == studentId ? f.Student2Id : f.Student1Id,
                    Name = f.Student1Id == studentId
                        ? f.Student2.User.FirstName + " " + f.Student2.User.LastName
                        : f.Student1.User.FirstName + " " + f.Student1.User.LastName,
                    ProfilePicture = f.Student1Id == studentId
                        ? f.Student2.User.ProfilePicture
                        : f.Student1.User.ProfilePicture,
                    Grade = f.Student1Id == studentId
                        ? f.Student2.Grade
                        : f.Student1.Grade,
                }).ToListAsync(cancellationToken);

            var sentRequests = await _context.Friends
                .AsNoTracking()
                .Where(f => f.Status == FriendStatus.Pending && f.ActionStudentId == studentId)
                .Select(f => new FriendRequestDto
                {
                    Id = f.Student1Id == studentId ? f.Student2Id : f.Student1Id,
                    Name = f.Student1Id == studentId
                        ? f.Student2.User.FirstName + " " + f.Student2.User.LastName
                        : f.Student1.User.FirstName + " " + f.Student1.User.LastName,
                    ProfilePicture = f.Student1Id == studentId
                        ? f.Student2.User.ProfilePicture
                        : f.Student1.User.ProfilePicture,
                    Grade = f.Student1Id == studentId
                        ? f.Student2.Grade
                        : f.Student1.Grade,
                }).ToListAsync(cancellationToken);

            return new RequestsDto
            {
                FriendRequests = incomingRequests,
                SentRequests = sentRequests
            };
        }
        public async Task<bool> RequestAlreadySentAsync(int studentSenderId, int studentRecevierId, CancellationToken cancellationToken)
        {
            var s1 = Math.Min(studentSenderId, studentRecevierId);
            var s2 = Math.Max(studentSenderId, studentRecevierId);
            return await _context.Friends.AnyAsync(f => f.Student1Id == s1 && f.Student2Id == s2, cancellationToken);
        }
        public async Task<Friend?> GetFriendRequestPendingAsync(int requestId, CancellationToken cancellationToken)
        {
            return await _context.Friends.FirstOrDefaultAsync(f => f.Id == requestId && f.Status == FriendStatus.Pending, cancellationToken);
        }
        public async Task UpdateRequestStatusAsync(Friend request, CancellationToken cancellationToken)
        {
            _context.Friends.Update(request);
        }
    }
}