using Application.Features.Attempts.Dtos;
using Application.Features.Quizzes.Dtos;
using Application.Features.StudentQuizes.Dtos;
using Domain.Enums;

namespace Infrastructure.Repositories
{
    internal sealed class QuizRepository : IQuizRepository
    {
        private readonly AppDbContext _dbContext;
        public QuizRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<AttemptDto>> GetAttempts(int courseId, int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.Students.Where(s => s.Enrollments.Any(c => c.CourseId == courseId))
                 .LeftJoin(_dbContext.QuizAttempts.Where(sv => sv.ModuleItemId == itemId),
                     student => student.Id,
                     quizAttempt => quizAttempt.StudentId,
                     (student, quizAttempt) => new AttemptDto
                     {
                         Id = quizAttempt != null ? quizAttempt.Id : 0,
                         StudentId = student.Id,
                         StudentName = student.User.FirstName + " " + student.User.LastName,
                         ProfilePicture = student.User.ProfilePicture,
                         SubmissionStatus = quizAttempt != null ? quizAttempt.SubmissionStatus : SubmissionStatus.NotStarted,
                         GradingStatus = quizAttempt != null ? quizAttempt.GradingStatus : GradingStatus.NotGraded,
                         SubmittedAt = quizAttempt != null ? quizAttempt.SubmittedAt : null,
                         StartedAt = quizAttempt != null ? quizAttempt.StartedAt : null,
                         TimeSpent = quizAttempt != null ? quizAttempt.TimeSpent : null,
                         Score = quizAttempt != null ? quizAttempt.Score : null,
                         TotalMarks = quizAttempt != null ? quizAttempt.TotalMarks : 0,
                         IsPassed = quizAttempt != null ? quizAttempt.Score >= quizAttempt.Quiz.PassingScore : null
                     }).ToListAsync(cancellationToken);
        }
        public async Task<QuizMetadata?> GetQuizMetadata(int quizId, int courseId, int moduleId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.ModuleItems
                .Where(mi => mi.Id == quizId && mi.ModuleId == moduleId && mi.CourseId == courseId && mi.Course.Tenant.SubDomain == subdomain)
                .Select(mi => new QuizMetadata
                {
                    Course = mi.Course.Title,
                    Module = mi.Module.Title,
                    Title = mi.Title,
                    Description = mi.Description,
                    Subject = mi.Course.Subject.Label,
                    Grade = mi.Course.Grade.Label
                }).FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<OverviewDto?> GetQuizOverview(int itemId, CancellationToken cancellationToken)
        {
            return await _dbContext.QuizAttempts.Where(sv => sv.ModuleItemId == itemId)
                .GroupBy(sv => 1)
                .Select(g => new OverviewDto
                {
                    TotalAttempts = g.Count(),
                    AverageScore = g.Average(a => a.Score) ?? 0,
                    HighestScore = g.Max(a => a.Score) ?? 0,
                    LowestScore = g.Min(a => a.Score) ?? 0,
                    AverageTime = g.Average(a => a.TimeSpent) ?? 0
                }).FirstOrDefaultAsync(cancellationToken);

        }
        public async Task<QuizQuestion?> GetQuizQuestion(int quizId, int quizQuestionId, string subdomain, CancellationToken cancellationToken)
        {
            return await _dbContext.QuizQuestions
                .Include(x => x.Question)
                .FirstOrDefaultAsync(q => q.QuizId == quizId && q.Id == quizQuestionId && q.Question.Tenant.SubDomain == subdomain, cancellationToken);
        }
        public async Task RemoveQuizQuestion(QuizQuestion quizQuestion, CancellationToken cancellationToken)
        {
            _dbContext.QuizQuestions.Remove(quizQuestion);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task<StudentQuizDto?> GetStudentQuizAsync(int studentId, int courseId, int itemId, CancellationToken cancellationToken)
        {
            var quizData = await _dbContext.ModuleItems
                .Where(mi => mi.Id == itemId && mi.CourseId == courseId)
                .Select(mi => new
                {
                    Quiz = new QuizDto
                    {
                        Id = mi.Id,
                        TotalMarks = mi.Quiz!.TotalMarks,
                        HasStarted = mi.Quiz.Attempts.Any(a => a.StudentId == studentId),
                        Duration = mi.Quiz.Duration,
                        StartDate = mi.Quiz.StartDate,
                        EndDate = mi.Quiz.EndDate,
                        CreatedAt = mi.CreatedAt,
                    },
                    Questions = mi.Quiz.Questions
                        .Select(q => new
                        {
                            q.Id,
                            q.Order,
                            q.Question.Type,
                            q.Question.QuestionTitle,
                            q.Marks,
                            q.Question.Difficulty,
                            q.Question.CorrectAnswer,
                            q.Question.Explanation,
                            q.Question.Options,
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (quizData is null)
                return null;

            var result = new StudentQuizDto
            {
                Quiz = quizData.Quiz,
                Questions = quizData.Questions.Select(q => new StudentQuestionDto
                {
                    Id = q.Id,
                    Order = q.Order,
                    Type = q.Type,
                    QuestionText = q.QuestionTitle,
                    Marks = q.Marks,
                    Difficulty = q.Difficulty,
                    CorrectAnswer = q.CorrectAnswer,
                    Explanation = q.Explanation,
                    Options = q.Options?.Select(o => new QuestionOptionDto
                    {
                        Id = o.Id,
                        Label = o.Label,
                    }).ToList(),
                    Answer = null
                }).ToList()
            };

            var attemptData = await _dbContext.QuizAttempts
                .Where(qa => qa.ModuleItemId == itemId && qa.StudentId == studentId)
                .Select(qa => new
                {
                    qa.Id,
                    qa.GradingStatus,
                    qa.StartedAt,
                    qa.SubmittedAt,
                    qa.TimeSpent,
                    qa.Score,
                    qa.TotalMarks,
                    TotalQuestions = qa.Quiz.Questions.Count(),
                    Answers = qa.Answers.Select(a => new
                    {
                        a.QuizQuestionId,
                        a.StudentAnswer,
                        a.IsCorrect,
                        a.AutoScore,
                        a.Feedback
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (attemptData != null)
            {
                result.Attempt = new StudentAttemptDto
                {
                    Id = attemptData.Id,
                    GradingStatus = attemptData.GradingStatus,
                    StartedAt = attemptData.StartedAt,
                    SubmittedAt = attemptData.SubmittedAt,
                    TimeSpent = attemptData.TimeSpent,
                    Score = attemptData.Score,
                    MaxScore = attemptData.TotalMarks,
                    IsPublished = attemptData.GradingStatus == GradingStatus.Published,
                    Summary = new SummaryDto
                    {
                        Correct = attemptData.Answers.Count(a => a.IsCorrect),
                        Wrong = attemptData.Answers.Count(a => !a.IsCorrect),
                        Skipped = attemptData.TotalQuestions - attemptData.Answers.Count
                    }
                };

                foreach (var question in result.Questions)
                {
                    var answer = attemptData.Answers.FirstOrDefault(a => a.QuizQuestionId == question.Id);
                    if (answer != null)
                    {
                        question.Answer = new QuestionAnswerDto
                        {
                            Value = answer.StudentAnswer,
                            IsCorrect = answer.IsCorrect,
                            Score = answer.AutoScore,
                            Feedback = answer.Feedback
                        };
                    }
                }
            }
            return result;
        }
    }
}