namespace Application.Features.Tenants.Dtos
{
    public sealed class LiveSessionSettingsDto
    {
        public bool EnableChat { get; set; }
        public bool ParticipantVideo { get; set; }
        public bool WaitingRoom { get; set; }
    }
}