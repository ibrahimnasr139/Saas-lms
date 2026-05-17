using Application.Features.Questions.Dtos;

namespace Application.Contracts.Repositories
{
    public interface IQuestionRepository
    {
        Task CreateQuestionCategory(QuestionCategory category, CancellationToken cancellationToken);
        Task<bool> IsUniqueCategory(string title, string subdomain, CancellationToken cancellationToken);
        Task<QuestionCategory?> GetQuestionCategory(string title, string subdomain, CancellationToken cancellationToken);
        Task<bool> IsFoundCategory(int id, CancellationToken cancellationToken);
        Task<Question?> GetQuestion(int id, string subdomain, CancellationToken cancellationToken);
        Task RemoveAsync(Question question, CancellationToken cancellationToken);
        Task<int> CreateQuestion(Question question, CancellationToken cancellationToken);
        Task<List<QuestionCategoryDto>> GetQuestionWithCategory(string subdomain, CancellationToken cancellationToken);
        Task<IEnumerable<AllQuestionsDto>> GetAllQuestions(string subdomain, CancellationToken cancellationToken);
        Task<int> GetTotalQuestions(string subdomain, CancellationToken cancellationToken);
        Task ReorderQuestions(int quizId, Dictionary<int, int> quizQuestionIds, CancellationToken cancellationToken);
        Task<int> GetUsedQuestions(string subdomain, CancellationToken cancellationToken);
        Task<int> GetWeekQuestions(string subdomain, CancellationToken cancellationToken);
        Task IncreaseReuse(IEnumerable<int> questionIds, CancellationToken cancellationToken);
        Task AddQuestionsToQuiz(IEnumerable<QuizQuestion> quizQuestions, CancellationToken cancellationToken);
        Task<IEnumerable<QuestionTypeDto>> GetQuestionsByType(string subdomain, CancellationToken cancellationToken);
        Task<IEnumerable<CategoryStatisticsDto>> GetQuestionCategories(string subdomain, CancellationToken cancellationToken);
        Task<int> GetLastQuestionOrderAsync(int quizId, CancellationToken cancellationToken);
    }
}