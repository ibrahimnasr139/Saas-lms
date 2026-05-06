using System.Net;

namespace Domain.Errors
{
    public static class CourseErrors
    {
        public static Error CourseNotFound => new(
            "Course.NotFound",
            "هذا الكورس غير موجود",
            HttpStatusCode.NotFound
        );

    }
}
