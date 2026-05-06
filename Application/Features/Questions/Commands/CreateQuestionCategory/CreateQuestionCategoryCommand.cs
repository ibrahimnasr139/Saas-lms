namespace Application.Features.Questions.Commands.CreateQuestionCategory
{
    public sealed record CreateQuestionCategoryCommand(string Title) : IRequest<OneOf<bool, Error>>;
}
