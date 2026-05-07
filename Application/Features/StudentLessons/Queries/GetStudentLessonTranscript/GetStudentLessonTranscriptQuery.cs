using Application.Features.StudentLessons.Dtos;

namespace Application.Features.StudentLessons.Queries.GetStudentLessonTranscript
{
    public sealed record GetStudentLessonTranscriptQuery(int CourseId, int ItemId) : IRequest<OneOf<StudentLessonTranscriptDto, Error>>;
}