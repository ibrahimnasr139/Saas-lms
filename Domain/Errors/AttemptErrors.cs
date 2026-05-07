using System.Net;

namespace Domain.Errors
{
    public static class AttemptErrors
    {
        public static Error AttemptNotFound =>
            new("Attempt.NotFound", "المحاولة غير موجودة", HttpStatusCode.NotFound);

        public static Error AttemptAlreadyPublished =>
            new("Attempt.AlreadyPublished", "تم نشر هذه المحاولة بالفعل", HttpStatusCode.BadRequest);

        public static Error AttemptNotGraded =>
            new("Attempt.NotGraded", "لا يمكن نشر هذه المحاولة لأنها لم يتم تصحيحها بعد", HttpStatusCode.BadRequest);

        public static Error AttemptAlreadyExists =>
            new("Attempt.AlreadyExists", "لقد قمت بمحاولة هذا الاختبار من قبل", HttpStatusCode.BadRequest);
    }
}