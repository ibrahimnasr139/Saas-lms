using System.Net;

namespace Domain.Errors
{
    public static class ModuleItemErrors
    {
        public static Error ModuleItemNotFound =>
            new Error("ModuleItem.NotFound", "هذا العنصر غير موجود", HttpStatusCode.NotFound);
        public static Error ModuleItemLocked =>
            new Error("ModuleItem.Locked", "هذا العنصر مقفول، يجب إتمام المتطلبات السابقة أولاً", HttpStatusCode.Forbidden);
        public static Error ModuleItemAlreadyCompleted =>
            new Error("ModuleItem.AlreadyCompleted", "لقد قمت بتسليم هذا الواجب مسبقاً", HttpStatusCode.Conflict);
    }
}