using Domain.Enums;

namespace Application.Features.Assignments.Commands.UpdateAssignment
{
    public sealed record UpdateAssignmentCommand(int CourseId, int ModuleId, int ItemId, DateTime DueDate, string Instructions,
        int TotalMarks, SubmissionType SubmissionType, IEnumerable<Attachment> Attachments) : IRequest<OneOf<SuccessDto, Error>>;
}