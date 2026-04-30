using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entites
{
    public sealed class Question : IAuditable
    {
        public int Id { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public Difficulty Difficulty { get; set; }
        public int Reuse { get; set; }
        public int? QuestionCategoryId { get; set; }
        public QuestionCategory? QuestionCategory { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public List<QuestionOption>? Options { get; set; }
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
    public sealed class QuestionCategory
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public ICollection<Question> Questions { get; set; } = [];
    }
    public sealed class QuestionOption
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
