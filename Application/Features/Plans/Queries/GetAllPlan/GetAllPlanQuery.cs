using Application.Features.Plans.Dtos;
namespace Application.Features.Plans.Queries.GetAllPlan
{
    public sealed record GetAllPlanQuery : IRequest<PlansDto>;
}