using Application.Features.Lessons.Dtos;

namespace Application.Features.Lessons.Commands.UpdateLesson
{
    public sealed record UpdateLessonCommand(int CourseId, int ModuleId, int ItemId, string VideoId, List<ResourceDto> Resources)
        : IRequest<OneOf<SuccessDto, Error>>;
}
