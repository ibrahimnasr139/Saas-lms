using System.Net;

namespace Domain.Errors
{
    public static class StudentSubscriptionErrors
    {
        public static Error StudentSubscribedExpired =>
            new Error("StudentSubscribed.Expired", "انتهى اشتراك هذا الطالب في هذا الكورس ... يرجي التجديد", HttpStatusCode.Forbidden);
    }
}