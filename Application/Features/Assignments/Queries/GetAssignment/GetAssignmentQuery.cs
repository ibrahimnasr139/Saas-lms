using Application.Features.ModuleItems.Dtos;

namespace Application.Features.Assignments.Queries.GetAssignment
{
    public sealed record GetAssignmentQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<AssignmentDto, Error>>;
}
