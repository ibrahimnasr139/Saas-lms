using Application.Features.Questions.Dtos;

namespace Application.Features.Questions.Queries.GetQuestion
{
    public sealed record GetQuestionQuery(int QuestionId) : IRequest<OneOf<SingleQuestionDto, Error>>;
}
