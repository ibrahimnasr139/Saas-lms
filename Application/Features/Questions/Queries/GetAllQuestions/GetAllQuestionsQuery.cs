using Application.Features.Questions.Dtos;

namespace Application.Features.Questions.Queries.GetAllQuestions
{
    public sealed record GetAllQuestionsQuery : IRequest<IEnumerable<AllQuestionsDto>>;
}
