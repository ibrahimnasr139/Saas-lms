using System.Net;

namespace Domain.Errors
{
    public static class SubjectErrors
    {
        public static Error SubjectNotFound =>
            new Error("Subject.SubjectNotFound", "المادة الدراسية غير موجودة", HttpStatusCode.NotFound);

        public static Error ChapterNotFound =>
            new Error("Chapter.ChapterNotFound", "الفصل غير موجود", HttpStatusCode.NotFound);
    }
}