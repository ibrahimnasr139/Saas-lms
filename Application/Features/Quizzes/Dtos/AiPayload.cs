namespace Application.Features.Quizzes.Dtos
{
    public sealed record AiPayload(string? Prompt, QuizMetadata Metadata, string Difficulty, string Type, int QuestionsNumber);
}