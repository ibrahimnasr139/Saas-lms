using System.Net;

namespace Domain.Errors
{
    public static class ModuleItemErrors
    {
        public static Error ModuleItemNotFound =>
            new Error("ModuleItem.NotFound", "هذا العنصر غير موجود", HttpStatusCode.NotFound);
    }
}
