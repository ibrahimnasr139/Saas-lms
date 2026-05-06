namespace Domain.Entites
{
    public sealed class QuizQuestion
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public int Marks { get; set; }
        public int Order { get; set; }
        public bool RequiresManualGrading { get; set; }
        public ICollection<Answer> Answers { get; set; } = [];
    }
}
