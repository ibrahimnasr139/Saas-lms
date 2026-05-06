using System.Net;

namespace Domain.Errors
{
    public static class ZoomError
    {
        public static Error ZoomTokenRefreshFailed =>
           new("Zoom.TokenRefreshFailed", "فشل في تحديث رمز الوصول الخاص بـ Zoom. يُرجى إعادة ربط حسابك.", HttpStatusCode.Unauthorized);

        public static Error ZoomAccountNotConnected =>
           new("Zoom.AccountNotConnected", "يرجى ربط حساب Zoom الخاص بك أولاً.", HttpStatusCode.BadRequest);

        public static Error ZoomMeetingCreationFailed =>
           new("Zoom.MeetingCreationFailed", "فشل في إنشاء اجتماع Zoom. حاول مرة أخرى لاحقًا.", HttpStatusCode.BadRequest);

        public static Error ZoomMeetingUpdateFailed =>
            new("Zoom.MeetingUpdateFailed", "حدث خطأ أثناء تحديث اجتماع Zoom", HttpStatusCode.InternalServerError);

        public static Error ZoomMeetingDeleteFailed =>
            new("Zoom.MeetingUpdateFailed", "حدث خطأ أثناء حذف اجتماع Zoom", HttpStatusCode.InternalServerError);

    }
}
