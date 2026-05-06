namespace Application.Features.Tenants.Dtos
{
    public sealed class ContentLibraryStatisticsDto
    {
        public int TotalFiles { get; set; }
        public int TotalVideos { get; set; }
        public int TotalImages { get; set; }
        public int TotalDocuments { get; set; }
    }
}
