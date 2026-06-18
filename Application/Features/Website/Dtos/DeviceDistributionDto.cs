using Domain.Enums;

namespace Application.Features.Website.Dtos
{
    public sealed class DeviceDistributionDto
    {
        public DeviceType DeviceType { get; set; }
        public int Visitors { get; set; }
    }
}