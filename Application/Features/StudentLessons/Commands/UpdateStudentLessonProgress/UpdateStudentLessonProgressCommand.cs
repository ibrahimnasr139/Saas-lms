using Application.Features.StudentLessons.Dtos;

namespace Application.Features.StudentLessons.Commands.UpdateStudentLessonProgress
{
    public sealed record UpdateStudentLessonProgressCommand(int CourseId, int ItemId, string VideoId, List<double[]> Segments,
        int LastPosition, int Duration) : IRequest<OneOf<LessonProgressDto, Error>>;
}