namespace Domain.Entites
{
    public sealed class Quiz
    {
        public int Duration { get; set; }
        public int PassingScore { get; set; }
        public int TotalMarks { get; set; }
        public bool ShowCorrectAnswers { get; set; }
        public bool ShuffleQuestions { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ModuleItemId { get; set; }
        public ModuleItem ModuleItem { get; set; } = null!;
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public string CreatedById { get; set; } = string.Empty;
        public ApplicationUser CreatedBy { get; set; } = null!;
        public ICollection<QuizQuestion> Questions { get; set; } = [];
        public ICollection<QuizAttempt> Attempts { get; set; } = [];
    }
}
