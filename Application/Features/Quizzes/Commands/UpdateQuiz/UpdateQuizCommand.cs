namespace Application.Features.Quizzes.Commands.UpdateQuiz
{
    public sealed record UpdateQuizCommand(int ModuleId, int CourseId, int ItemId, int Duration, int PassingScore, bool ShowCorrectAnswers, bool ShuffleQuestions, TimeOnly StartTime,
         TimeOnly EndTime, DateTime StartDate, DateTime EndDate) : IRequest<OneOf<SuccessDto, Error>>;
}
