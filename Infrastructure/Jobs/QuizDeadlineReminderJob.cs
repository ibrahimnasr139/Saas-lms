using Application.Constants;
using Application.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Jobs
{
    public sealed class QuizDeadlineReminderJob
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<QuizDeadlineReminderJob> _logger;

        public QuizDeadlineReminderJob(IQuizRepository quizRepository, IEmailSender emailSender,
            ILogger<QuizDeadlineReminderJob> logger)
        {
            _quizRepository = quizRepository;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task SendQuizDeadlineRemindersAsync()
        {
            var reminders = await _quizRepository.GetQuizzesEndingWithin24HoursAsync(CancellationToken.None);
            foreach (var reminder in reminders)
            {
                try
                {
                    var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                        EmailConstants.QuizDeadlineReminderTemplate
                        , new Dictionary<string, string>
                        {
                            { "{{StudentName}}", reminder.StudentName },
                            { "{{CourseTitle}}", reminder.CourseTitle },
                            { "{{QuizTitle}}", reminder.QuizTitle },
                            { "{{EndDate}}", reminder.EndDate.ToString("yyyy-MM-dd HH:mm") + " UTC" },
                            { "{{QuizUrl}}", $"{EmailConstants.CourseLink}/{reminder.CourseId}" }
                        }
                    );
                    await _emailSender.SendEmailAsync(reminder.StudentEmail, "تذكير: موعد انتهاء الكويز قريب", emailBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send quiz deadline reminder to {Email}", reminder.StudentEmail);
                }
            }
        }
    }
}