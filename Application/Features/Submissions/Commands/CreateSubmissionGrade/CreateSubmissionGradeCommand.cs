namespace Application.Features.Submissions.Commands.CreateSubmissionGrade
{
    public sealed record CreateSubmissionGradeCommand(int CourseId, int ModuleId, int ItemId, int SubmissionId, double Grade,
        string? Feedback) : IRequest<OneOf<bool, Error>>;
}