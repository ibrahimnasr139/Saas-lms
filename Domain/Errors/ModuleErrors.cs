using System.Net;

namespace Domain.Errors
{
    public static class ModuleErrors
    {
        public static Error ModuleNotFound => new(
            "Module.NotFound",
            "هذا الموديول غير موجود",
            HttpStatusCode.NotFound
        );

    }
}
