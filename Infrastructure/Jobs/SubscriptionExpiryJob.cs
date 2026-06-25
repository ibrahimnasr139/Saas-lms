using Application.Constants;
using Application.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Jobs
{
    public sealed class SubscriptionExpiryJob
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<SubscriptionExpiryJob> _logger;
        public SubscriptionExpiryJob(ISubscriptionRepository subscriptionRepository, IEmailSender emailSender,
            ILogger<SubscriptionExpiryJob> logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _emailSender = emailSender;
            _logger = logger;
        }
        public async Task ProcessSubscriptionExpiryAsync()
        {
            await ExpireSubscriptionsAsync();
            await SendExpiringSoonRemindersAsync();
        }
        private async Task ExpireSubscriptionsAsync()
        {
            var expired = await _subscriptionRepository.GetExpiredSubscriptionsAsync(CancellationToken.None);
            if (expired.Count == 0) return;

            var ids = expired.Select(s => s.SubscriptionId).ToList();
            await _subscriptionRepository.BulkExpireSubscriptionsAsync(ids, CancellationToken.None);

            foreach (var subscription in expired)
            {
                try
                {
                    var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                        EmailConstants.SubscriptionExpiredTemplate,
                        new Dictionary<string, string>
                        {
                            { "{{TenantName}}", subscription.TenantName },
                            { "{{RenewalUrl}}", EmailConstants.RenewalLink }
                        });
                    await _emailSender.SendEmailAsync(subscription.TenantEmail, "انتهى اشتراكك في Waey", emailBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send expiry email to {Email}", subscription.TenantEmail);
                }
            }
        }
        private async Task SendExpiringSoonRemindersAsync()
        {
            var expiringSoon = await _subscriptionRepository.GetSubscriptionsExpiringSoonAsync(CancellationToken.None);

            foreach (var subscription in expiringSoon)
            {
                try
                {
                    var emailBody = EmailConfirmationHelper.GenerateEmailBodyHelper(
                        EmailConstants.SubscriptionExpiringSoonTemplate,
                        new Dictionary<string, string>
                        {
                            { "{{TenantName}}", subscription.TenantName },
                            { "{{EndDate}}", subscription.EndsAt.ToString("yyyy-MM-dd") },
                            { "{{RenewalUrl}}", EmailConstants.RenewalLink }
                        });
                    await _emailSender.SendEmailAsync(subscription.TenantEmail, "تذكير: اشتراكك على وشك الانتهاء", emailBody);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send expiry reminder to {Email}", subscription.TenantEmail);
                }
            }
        }
    }
}