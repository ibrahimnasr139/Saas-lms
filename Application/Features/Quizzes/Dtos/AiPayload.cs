using Domain.Enums;

namespace Application.Features.Quizzes.Dtos
{
    public sealed record AiPayload(string? Prompt, QuizMetadata Metadata, Difficulty Difficulty, QuestionType Type,
        int QuestionsNumber);
}
