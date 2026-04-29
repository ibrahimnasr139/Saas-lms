using Application.Features.TenantMembers.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal class TenantInviteRepository : ITenantInviteRepository
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public TenantInviteRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateTenantInviteAsync(TenantInvite tenantInvite, CancellationToken cancellationToken)
        {
            await _context.TenantInvites.AddAsync(tenantInvite, cancellationToken);
        }
        public async Task<ValidateTenanInviteDto> GetValidateTenanInviteAsync(string token, CancellationToken cancellationToken)
        {
            var result = await _context.TenantInvites
                .AsNoTracking()
                .Where(ti => ti.Token == token)
                .ProjectTo<ValidateTenanInviteDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
            return result!;
        }
        public Task<bool> IsValidTokenAsync(string token, CancellationToken cancellationToken)
        {
            return _context.TenantInvites
                .AsNoTracking()
                .AnyAsync(ti => ti.Token == token && ti.ExpiresAt > DateTime.UtcNow && ti.AcceptedAt == null, cancellationToken);
        }
        public Task<string> GetInvitedMemberEmailAsync(string token, CancellationToken cancellationToken)
        {
            return _context.TenantInvites
                .AsNoTracking()
                .Where(ti => ti.Token == token)
                .Select(ti => ti.Email)
                .FirstOrDefaultAsync(cancellationToken)!;
        }
        public Task<TenantInvite?> GetInviteByTokenAsync(string token, CancellationToken cancellationToken)
        {
            return _context.TenantInvites
                .AsNoTracking()
                .FirstOrDefaultAsync(ti => ti.Token == token, cancellationToken);
        }
        public async Task AcceptInviteAsync(string token, CancellationToken cancellationToken)
        {
            await _context.TenantInvites
                .Where(ti => ti.Token == token)
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(ti => ti.AcceptedAt, DateTime.UtcNow)
                    .SetProperty(ti => ti.Status, TenantInviteStatus.Accepted),cancellationToken);
        }
        public Task DeclineInviteAsync(string token, CancellationToken cancellationToken)
        {
            return _context.TenantInvites
                .Where(ti => ti.Token == token)
                .ExecuteUpdateAsync(updates => updates
                    .SetProperty(ti => ti.Status, TenantInviteStatus.Declined), cancellationToken);
        }
    }
}
