using Application.Constants;
using Application.Features.Public.Dtos;
using Application.Features.TenantWebsite.Commands.CreateTenantPage;
using Application.Features.TenantWebsite.Commands.UpdateTenantPage;
using Application.Features.TenantWebsite.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class TenantPageRepository : ITenantPageRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TenantPageRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateTenantPageAsync(CreateTenantPageCommand request, int tenantId, CancellationToken cancellationToken)
        {
            var tenantPage = new TenantPage
            {
                Title = request.Title,
                Url = request.Url,
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                Status = request.Status,
                TenantId = tenantId,
                PageBlocks = request.PageBlocks.Select(pb => new PageBlock
                {
                    BlockTypeId = pb.BlockType,
                    Order = pb.Order,
                    Visible = pb.Visible,
                    Props = pb.Props,
                }).ToList()
            };
            await _context.AddAsync(tenantPage, cancellationToken);
        }
        public async Task<int> DeleteTenantPageAsync(int tenantId, int pageId, CancellationToken cancellationToken)
        {
            return await _context.TenantPages.Where(tp => tp.Id == pageId && tp.TenantId == tenantId).ExecuteDeleteAsync(cancellationToken);
        }
        public async Task DuplicateTenantPageAsync(TenantPage tenantPage, CancellationToken cancellationToken)
        {
            await _context.AddAsync(tenantPage, cancellationToken);
        }
        public Task<List<TenantPagesDto>> GetTenantPagesAsync(int tenantId, CancellationToken cancellationToken)
        {
            return _context.TenantPages
                .AsNoTracking()
                .Where(tp => tp.TenantId == tenantId)
                .ProjectTo<TenantPagesDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<TenantPage?> GetTenantPageAsync(int tenantId, int pageId, CancellationToken cancellationToken)
        {
            return await _context.TenantPages
                .AsNoTracking()
                .Include(p => p.PageBlocks)
                .FirstOrDefaultAsync(p => p.Id == pageId && p.TenantId == tenantId, cancellationToken);
        }
        public async Task<TenantPageBlocksDto?> GetBlocksTypeAsync(CancellationToken cancellationToken)
        {
            var blockTypes = await _context.BlockTypes
                .AsNoTracking()
                .ProjectTo<BlockTypeDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new TenantPageBlocksDto
            {
                Hero = blockTypes.FirstOrDefault(b => b.Id == TenantWebsiteConstants.HeroId) ?? new(),
                Text = blockTypes.FirstOrDefault(b => b.Id == TenantWebsiteConstants.TextId) ?? new(),
                Featured_courses = blockTypes.FirstOrDefault(b => b.Id == TenantWebsiteConstants.Featured_CoursesId) ?? new(),
                Testimonials = blockTypes.FirstOrDefault(b => b.Id == TenantWebsiteConstants.TestimonialsId) ?? new(),
                Cta = blockTypes.FirstOrDefault(b => b.Id == TenantWebsiteConstants.CtaId) ?? new(),
                Footer = blockTypes.FirstOrDefault(b => b.Id == TenantWebsiteConstants.FooterId) ?? new(),
            };
        }
        public async Task<bool> UrlExistsAsync(int tenantId, string url, CancellationToken cancellationToken)
        {
            return await _context.TenantPages
                .AnyAsync(p => p.TenantId == tenantId && p.Url == url, cancellationToken);
        }
        public async Task<bool> UpdateTenantPageAsync(int pageId, int tenantId, UpdateTenantPageCommand update, CancellationToken cancellationToken)
        {
            var existingPage = await _context.TenantPages
                 .Include(p => p.PageBlocks)
                .FirstOrDefaultAsync(p => p.Id == pageId && p.TenantId == tenantId, cancellationToken);
            if (existingPage == null)
                return false;

            existingPage.Title = update.Title;
            existingPage.Url = update.Url;
            existingPage.Status = update.Status;
            existingPage.MetaTitle = update.MetaTitle;
            existingPage.MetaDescription = update.MetaDescription;

            _context.PageBlocks.RemoveRange(existingPage.PageBlocks);

            existingPage.PageBlocks = update.PageBlocks.Select(pb => new PageBlock
            {
                BlockTypeId = pb.BlockType,
                Order = pb.Order,
                Visible = pb.Visible,
                Props = pb.Props
            }).ToList();

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<TenantPageDto?> GetTenantPageWithBlockTypeAsync(int tenantId, int pageId, CancellationToken cancellationToken)
        {
            var tenantPage = await _context.TenantPages
                .AsNoTracking()
                .Include(p => p.PageBlocks)
                    .ThenInclude(pb => pb.BlockType)
                .FirstOrDefaultAsync(p => p.Id == pageId && p.TenantId == tenantId, cancellationToken);

            if (tenantPage == null) return null;

            return new TenantPageDto
            {
                Page = _mapper.Map<TenantPagesDto>(tenantPage),
                Blocks = tenantPage.PageBlocks.Select(pb => _mapper.Map<TenantBlockTypeDto>(pb)).ToList()
            };
        }
        public Task<List<TenantCourseDto>> GetTenantWebsiteCoursesAsync(string subDomain, List<int> courseIds, CancellationToken cancellationToken)
        {
            var courses = _context.Courses
                .AsNoTracking()
                .Where(c => c.Tenant.SubDomain == subDomain);

            if (courseIds.Any())
                courses = courses.Where(c => courseIds.Contains(c.Id));

            return courses
                .ProjectTo<TenantCourseDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public Task<List<TenantNavigationLinkDto>> GetTenantNavigationLinksAsync(int tenantId, CancellationToken cancellationToken)
        {
            return _context.TenantPages
                .AsNoTracking()
                .Where(tp => tp.TenantId == tenantId && tp.Status == TenantPageStatus.published)
                .ProjectTo<TenantNavigationLinkDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
        public async Task<TenantPageDto?> GetPublishedTenantPagesAsync(string url, string subDomain, CancellationToken cancellationToken)
        {
            var tenantPage = await _context.TenantPages
                .AsNoTracking()
                .Include(p => p.PageBlocks)
                    .ThenInclude(pb => pb.BlockType)
                .FirstOrDefaultAsync(tp => tp.Url == url && tp.Tenant.SubDomain == subDomain && tp.Status == TenantPageStatus.published, cancellationToken);

            if (tenantPage is null) return null;

            return new TenantPageDto
            {
                Page = _mapper.Map<TenantPagesDto>(tenantPage),
                Blocks = _mapper.Map<List<TenantBlockTypeDto>>(tenantPage.PageBlocks)
            };
        }
    }
}