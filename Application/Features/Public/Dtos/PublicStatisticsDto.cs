namespace Application.Features.Public.Dtos
{
    public sealed class PublicStatisticsDto
    {
        public WebsiteScorecardsDto WebsiteScorecards { get; set; } = new WebsiteScorecardsDto();
        public List<VisitorsAndPageViewsDataDto> VisitorsAndPageViewsData { get; set; } = new List<VisitorsAndPageViewsDataDto>();
        public List<MonthlyRevenueDto> MonthlyRevenueData { get; set; } = new List<MonthlyRevenueDto>();
        public List<TopPagesDto> TopPagesData { get; set; } = new List<TopPagesDto>();
        public List<DeviceDistributionDto> DeviceDistributionData { get; set; } = new List<DeviceDistributionDto>();
    }
}