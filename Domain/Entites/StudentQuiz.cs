using Domain.Enums;

namespace Domain.Entites
{
    public sealed class StudentQuiz
    {
        public int Id { get; set; }
        public int TimeLimit { get; set; }
        public StudentQuizDifficulty Difficulty { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int SubjectId { get; set; }
        public StudentSubject Subject { get; set; } = null!;
        public int? ChapterId { get; set; }
        public StudentChapter? Chapter { get;set; }
        public ICollection<StudentQuizAttempt> StudentQuizAttempts { get; set; } = [];
        public ICollection<StudentQuizQuestion> StudentQuizQuestions { get; set; } = [];
    }
}