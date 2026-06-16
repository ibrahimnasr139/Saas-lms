using Application.Features.Plan.DTOs;

namespace Application.Features.Plans.Dtos
{
    public sealed class PlansDto
    {
        public IEnumerable<PlanDto> Plans { get; init; } = [];
    }
}