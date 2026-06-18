using Domain.Enums;

namespace Application.Features.Public.Dtos
{
    public sealed class DeviceDistributionDto
    {
        public DeviceType DeviceType { get; set; }
        public int Visitors { get; set; }
        public string Fill { get; set; } = string.Empty;
    }
}