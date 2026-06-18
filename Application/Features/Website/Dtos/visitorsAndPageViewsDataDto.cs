namespace Application.Features.Website.Dtos
{
    public sealed class VisitorsAndPageViewsDataDto
    {
        public int Visitors { get; set; }
        public int PageViews { get; set; }
        public string Month { get; set; } = string.Empty;
    }
}