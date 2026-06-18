namespace Application.Features.Website.Dtos
{
    public sealed class EmailSettingsDto
    {
        public bool OrderSubmitted { get; set; }
        public bool OrderApproved { get; set; }
        public bool OrderRejected { get; set; }
    }
}
