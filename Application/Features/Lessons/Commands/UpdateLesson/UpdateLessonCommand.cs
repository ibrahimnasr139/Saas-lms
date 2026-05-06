namespace Application.Features.Lessons.Commands.UpdateLesson
{
    public sealed record UpdateLessonCommand(int CourseId, int ModuleId, int ItemId, string Title, string? Description, string VideoId,
        IEnumerable<Resource> Resources) : IRequest<OneOf<SuccessDto, Error>>;
}
