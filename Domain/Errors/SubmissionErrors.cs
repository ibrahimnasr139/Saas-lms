using System.Net;

namespace Domain.Errors
{
    public static class SubmissionErrors
    {
        public static Error SubmissionNotFound =>
            new("Submission.NotFound", "التقديم غير موجود", HttpStatusCode.NotFound);
    }
}