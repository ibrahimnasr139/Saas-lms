using Application.Features.ModuleItems.Dtos;
using Domain.Enums;

namespace Application.Features.ModuleItems.Commands.CreateModuleItem
{
    public sealed record CreateModuleItemCommand(int CourseId, int ModuleId, ModuleItemType Type, string Title, CourseStatus Status, bool AllowDiscussions,
        IEnumerable<ConditionDto> Conditions, string? Description, string? VideoId, IEnumerable<Resource>? Resources,
        DateTime? DueDate, string? Instructions, int? TotalMarks, SubmissionType? SubmissionType,
        IEnumerable<Attachment>? Attachments, int? Duration, int? PassingScore, bool? ShowCorrectAnswers, bool? ShuffleQuestions,
        DateTime? StartDate, DateTime? EndDate, TimeOnly? StartTime, TimeOnly? EndTime) : IRequest<OneOf<SuccessDto, Error>>;
}