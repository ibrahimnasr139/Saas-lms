using Application.Features.Website.Dtos;

namespace Application.Features.Website.Queries.GetTenantPageBlocks
{
    internal sealed class GetTenantPageBlocksQueryHandler : IRequestHandler<GetTenantPageBlocksQuery, TenantPageBlocksDto>
    {
        private readonly ITenantPageRepository _tenantWebsiteRepository;
        public GetTenantPageBlocksQueryHandler(ITenantPageRepository tenantWebsiteRepository)
        {
            _tenantWebsiteRepository = tenantWebsiteRepository;
        }
        public async Task<TenantPageBlocksDto> Handle(GetTenantPageBlocksQuery request, CancellationToken cancellationToken)
        {
            var tenantPageBlocks = await _tenantWebsiteRepository.GetBlocksTypeAsync(cancellationToken);
            return tenantPageBlocks!;
        }
    }
}
