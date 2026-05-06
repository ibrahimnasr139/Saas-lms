namespace Application.Features.TenantWebsite.Dtos
{
    public sealed class TenantPageBlocksDto
    {
        public BlockTypeDto Hero { get; set; } = new();
        public BlockTypeDto Text { get; set; } = new();
        public BlockTypeDto Featured_courses { get; set; } = new();
        public BlockTypeDto Testimonials { get; set; } = new();
        public BlockTypeDto Cta { get; set; } = new();
        public BlockTypeDto Footer { get; set; } = new();
    }
}
