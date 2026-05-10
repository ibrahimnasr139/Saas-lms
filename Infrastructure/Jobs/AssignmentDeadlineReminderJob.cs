using Application.Constants;
using Application.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Jobs
{
    public sealed class AssignmentDeadlineReminderJob
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AssignmentDeadlineReminderJob> _logger;

        public AssignmentDeadlineReminderJob(IAssignmentRepository assignmentRepository, IEmailSender emailSender,
            ILogger<AssignmentDeadlineReminderJob> logger)
        {
            _assignmentRepository = assignmentRepository;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task SendAssignmentDeadlineRemindersAsync()
        {
            var reminders = await _assignmentRepository.GetAssignmentsEndingWithin24HoursAsync(CancellationToken.None);
            foreach (var reminder in reminders)
            {
                try
                {
                    var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                        EmailConstants.AssignmentDeadlineReminderTemplate
                        , new Dictionary<string, string>
                        {
                            { "{{StudentName}}", reminder.StudentName },
                            { "{{CourseTitle}}", reminder.CourseTitle },
                            { "{{AssignmentTitle}}", reminder.AssignmentTitle },
                            { "{{DueDate}}", reminder.DueDate.ToString("yyyy-MM-dd HH:mm") + " UTC" },
                            { "{{AssignmentUrl}}", $"{EmailConstants.CourseLink}/{reminder.CourseId}" }
                        }
                    );
                    await _emailSender.SendEmailAsync(reminder.StudentEmail, "تذكير: موعد تسليم الواجب قريب", emailBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send assignment deadline reminder to {Email}", reminder.StudentEmail);
                }
            }
        }
    }
}