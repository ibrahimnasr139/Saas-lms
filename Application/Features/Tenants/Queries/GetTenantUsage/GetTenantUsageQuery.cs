using Application.Features.Tenants.Dtos;

namespace Application.Features.Tenants.Queries.GetTenantUsage
{
    public sealed record GetTenantUsageQuery : IRequest<TenantUsageDto>;
}
