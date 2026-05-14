using Domain.Enums;

namespace Application.Features.Courses.Commands.UpdateCourse
{
    public sealed record UpdateCourseCommand(int CourseId, string Title, string Description, int GradeId, int SubjectId, string[] Tags, string Thumbnail, string? Curriculum,
        decimal Price, PricingType PricingType, string Currency, byte Discount, CourseStatus Status, string Year, string? Video, string? Semester)
         : IRequest<OneOf<SuccessDto, Error>>;
}
