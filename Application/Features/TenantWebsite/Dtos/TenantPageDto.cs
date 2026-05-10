namespace Application.Features.TenantWebsite.Dtos
{
    public sealed class TenantPageDto
    {
        public TenantPagesDto Page { get; set; } = new TenantPagesDto();
        public List<TenantBlockTypeDto> Blocks { get; set; } = [];
    }
}