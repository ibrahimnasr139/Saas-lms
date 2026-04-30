using Application.Features.ModuleItems.Dtos;

namespace Application.Features.Quizzes.Queries.GetQuiz
{
    public sealed record GetQuizQuery(int CourseId, int ModuleId, int ItemId) : IRequest<OneOf<QuizDto, Error>>;
}