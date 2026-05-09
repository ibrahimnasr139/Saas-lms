using System.Net;

namespace Domain.Errors
{
    public static class StudentQuizErrors
    {
        public static Error NotFound =>
            new Error("StudentQuiz.NotFound", "الاختبار غير موجود.", HttpStatusCode.BadRequest);
    }
}