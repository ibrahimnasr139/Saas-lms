namespace Application.Features.Courses.Commands.DeleteCourse
{
    public sealed record DeleteCourseCommand(int CourseId)
         : IRequest<OneOf<SuccessDto, Error>>;
}
