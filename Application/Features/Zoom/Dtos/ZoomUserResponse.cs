namespace Application.Features.ZoomIntegration.Dtos
{
    public sealed class ZoomUserResponse
    {
        public string id { get; set; } = string.Empty;
        public string account_id { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public int type { get; set; }
        public string status { get; set; } = string.Empty;

    }
}
