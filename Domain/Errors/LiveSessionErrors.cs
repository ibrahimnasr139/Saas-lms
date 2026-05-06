using System.Net;

namespace Domain.Errors
{
    public static class LiveSessionErrors
    {
        public static Error SessionNotFound =>
            new("Session.NotFound", "تعذر العثور على الجلسة المطلوبة", HttpStatusCode.NotFound);

        public static Error CannotUpdateSession =>
            new("Session.CannotUpdate", "ليس لديك صلاحية لتحديث هذه الجلسة", HttpStatusCode.Forbidden);

        public static Error CannotUpdateLiveSession =>
            new("Session.CannotUpdateLive", "لا يمكن تعديل جلسة نشطة حالياً", HttpStatusCode.BadRequest);

        public static Error CannotUpdateEndedSession =>
            new("Session.CannotUpdateEnded", "لا يمكن تعديل جلسة انتهت بالفعل", HttpStatusCode.BadRequest);

        public static Error CannotDeleteActiveLiveSession =>
            new("Session.CannotDeleteActive", "لا يمكن حذف جلسة نشطة حالياً", HttpStatusCode.BadRequest);

        public static Error CannotDeleteOthersLiveSession =>
            new("Session.CannotDeleteNotOwner", "لا يمكنك حذف جلسة لم تقم بإنشائها.", HttpStatusCode.BadRequest);

        public static Error ZoomIntegrationNotAvailable =>
            new("Zoom.NotAvailable", "ميزة Zoom غير متاحة في باقتك الحالية", HttpStatusCode.Forbidden);
    }
}
