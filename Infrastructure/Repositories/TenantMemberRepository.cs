using Application.Constants;
using Application.Features.TenantMembers.Commands.UpdateCurrentMember;
using Application.Features.TenantMembers.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Infrastructure.Repositories
{
    internal sealed class TenantMemberRepository : ITenantMemberRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TenantMemberRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<string>?> GetAllPermissions(int tenantRoleId, CancellationToken cancellationToken)
        {
            return await _context.RolePermissions
                .AsNoTracking()
                .Where(trp => trp.TenantRoleId == tenantRoleId)
                .Select(trp => trp.PermissionId)
                .ToListAsync(cancellationToken);
        }
        public async Task<CurrentTenantMemberDto?> GetCurrentTenantMemberAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.TenantMembers
                 .AsNoTracking()
                 .ProjectTo<CurrentTenantMemberDto>(_mapper.ConfigurationProvider)
                 .FirstOrDefaultAsync(tm => tm.UserId == userId, cancellationToken);
        }
        public Task<List<TenantMembersDto>> GetTenantMembersAsync(int tenantId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .AsNoTracking()
                .Where(tm => tm.TenantId == tenantId)
                .ProjectTo<TenantMembersDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<bool> IsPermittedMember(string userId, string permission, CancellationToken cancellationToken)
        {
            var isPermitted = await _context.TenantMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(tm => tm.UserId == userId && (tm.TenantRole.HasAllPermissions || tm.TenantRole.RolePermissions.Any(p => p.PermissionId == permission)), cancellationToken);
            return isPermitted != null;
        }
        public Task<List<int>> GetTenantIdsAsync(string userId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .AsNoTracking()
                .Where(tm => tm.UserId == userId)
                .Select(tm => tm.TenantId)
                .ToListAsync(cancellationToken);
        }
        public Task<int> GetMemberIdByUserIdAsync(string userId, int tenantId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .AsNoTracking()
                .Where(tm => tm.UserId == userId && tm.TenantId == tenantId)
                .Select(tm => tm.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public Task<TenantMember?> GetMemberByIdAsync(int memberId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(tm => tm.Id == memberId, cancellationToken);
        }
        public async Task<bool> IsOwnerAsync(int memberId, CancellationToken cancellationToken)
        {
            return await _context.TenantMembers
                .AsNoTracking()
                .Where(tm => tm.Id == memberId)
                .Select(tm => tm.TenantRole.Name == TenantRoleConstants.Owner)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task RemoveMemberAsync(int memberId, CancellationToken cancellationToken)
        {
            await _context.TenantMembers
                .Where(tm => tm.Id == memberId)
                .ExecuteDeleteAsync(cancellationToken);
        }
        public Task UpdateRoleMemberAsync(int memberId, int roleId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .Where(tm => tm.Id == memberId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(tm => tm.TenantRoleId, roleId), cancellationToken);
        }
        public Task<MemberProfileDto> GetMemberProfileAsync(int memberId, CancellationToken cancellationToken)
        {
            var memberProfile = _context.TenantMembers
                .AsNoTracking()
                .Where(tm => tm.Id == memberId)
                .ProjectTo<MemberProfileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);
            return memberProfile!;
        }
        public async Task UpdateCurrentMemberAsync(int tenantId, int memberId, UpdateCurrentMemberCommand request, CancellationToken cancellationToken)
        {
            var member = await _context.TenantMembers
                .FirstOrDefaultAsync(tm => tm.Id == memberId, cancellationToken);

            member!.DisplayName = request.DisplayName ?? member.DisplayName;
            member.ExperienceYears = request.ExperienceYears ?? member.ExperienceYears;
            member.JobTitle = request.JobTitle ?? member.JobTitle;
            member.Bio = request.Bio ?? member.Bio;

            if (request.Subjects != null)
            {
                var existingSubjects = await _context.Subjects
                    .Where(s => s.TenantId == tenantId)
                    .ToListAsync(cancellationToken);

                var newValues = request.Subjects.Select(s => s.Value).ToHashSet();
                var toDelete = existingSubjects.Where(s => !newValues.Contains(s.Value)).ToList();
                _context.Subjects.RemoveRange(toDelete);

                var existingValues = existingSubjects.Select(s => s.Value).ToHashSet();
                var toAdd = request.Subjects
                    .Where(s => !existingValues.Contains(s.Value))
                    .Select(s => new Subject
                    {
                        TenantId = tenantId,
                        Label = s.Label,
                        Value = s.Value
                    });
                await _context.Subjects.AddRangeAsync(toAdd, cancellationToken);
            }

            if (request.TeachingLevels != null)
            {
                var existingLevels = await _context.TeachingLevels
                    .Where(t => t.TenantId == tenantId)
                    .ToListAsync(cancellationToken);

                var newValues = request.TeachingLevels.Select(t => t.Value).ToHashSet();
                var toDelete = existingLevels
                    .Where(t => !newValues.Contains(t.Value))
                    .ToList();
                _context.TeachingLevels.RemoveRange(toDelete);

                var existingValues = existingLevels.Select(t => t.Value).ToHashSet();
                var toAdd = request.TeachingLevels
                    .Where(t => !existingValues.Contains(t.Value))
                    .Select(t => new TeachingLevel
                    {
                        TenantId = tenantId,
                        Label = t.Label,
                        Value = t.Value
                    });
                await _context.TeachingLevels.AddRangeAsync(toAdd, cancellationToken);
            }

            if (request.Grades != null)
            {
                var existingGrades = await _context.Grades
                    .Where(g => g.TenantId == tenantId)
                    .ToListAsync(cancellationToken);

                var newValues = request.Grades.Select(g => g.Value).ToHashSet();
                var toDelete = existingGrades
                    .Where(g => !newValues.Contains(g.Value))
                    .ToList();
                _context.Grades.RemoveRange(toDelete);

                var existingValues = existingGrades.Select(g => g.Value).ToHashSet();
                var toAdd = request.Grades
                    .Where(g => !existingValues.Contains(g.Value))
                    .Select(g => new Grade
                    {
                        TenantId = tenantId,
                        Label = g.Label,
                        Value = g.Value
                    });
                await _context.Grades.AddRangeAsync(toAdd, cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<int> GetTenantmemberIdAsync(int tenantId, CancellationToken cancellationToken)
        {
            return await _context.TenantMembers
                .Where(tm => tm.TenantId == tenantId)
                .Select(tm => tm.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}