using Application.Features.Announcements.Dtos;
using AutoMapper;

namespace Infrastructure.Repositories
{
    internal class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AnnouncementRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateAnnouncementAsync(Announcement announcement, CancellationToken cancellationToken)
        {
            await _context.Announcements.AddAsync(announcement, cancellationToken);
        }
        public async Task<bool> DeleteAnnouncementAsync(int announcementId, string subDomain, CancellationToken cancellationToken)
        {
            var announcement = await _context.Announcements
                .FirstOrDefaultAsync(a => a.Id == announcementId && a.Tenant.SubDomain == subDomain);
            if (announcement is null)
                return false;

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<AllAnnouncements> GetAnnouncementsAsync(string subDomain, int Limit, string? Q, int Cursor, CancellationToken cancellationToken)
        {
            var query = _context.Announcements
                .AsNoTracking()
                .Where(a => a.Tenant.SubDomain == subDomain);

            if (!string.IsNullOrEmpty(Q))
                query = query.Where(a => a.Title.Contains(Q) || a.Content.Contains(Q));

            var announcements = await query
                .Where(a => a.Id < (Cursor == 0 ? int.MaxValue : Cursor))
                .OrderByDescending(dt => dt.Id)
                .Take(Limit + 1)
                .ToListAsync(cancellationToken);

            var data = _mapper.Map<List<AnnouncementDto>>(announcements);

            var hasMore = data.Count > Limit;
            if (hasMore)
                data.RemoveAt(data.Count - 1);

            return new AllAnnouncements
            {
                Data = data,
                HasMore = hasMore,
                NextCursor = data.LastOrDefault()?.Id ?? 0
            };
        }
    }
}