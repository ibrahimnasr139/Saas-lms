namespace Application.Constants
{
    public static class EmailConstants
    {
        public const string OtpTemplate = "OtpTemplate";
        public const string ForgetPasswordTemplate = "ForgetPasswordTemplate";
        public const string ResetPasswordSubject = "Reset Your Password";
        public const string EmailConfirmationSubject = "Email Confirmation";

        public const string ForgetPasswordUrl = "https://www.waey.online/auth/reset-password";
        public const string TenantMemberInviteUrl = "https://www.waey.online/invite";

        public const string TenantInviteTemplate = "TenantInviteTemplate";
        public const string LiveSessionTemplate = "LiveSessionTemplate";
        public const string UpdateLiveSessionTemplate = "UpdateLiveSessionTemplate";
        public const string CancelLiveSessionTemplate = "CancelLiveSessionTemplate";
        public const string Subject = "لقد تمت دعوتك للانضمام";
        public const string UpdateSubject = "تم تعديل موعد أو تفاصيل الجلسة";
        public const string DeleteSubject = "تم حذف موعد أو تفاصيل الجلسة";
        public const string StudentReminderSubject = "تذكير بالمهام القادمة";

        public const string CourseInviteTemplate = "CourseInviteTemplate";
        public const string CourseInviteUrl = "https://students.waey.online/course-invite";

        public const string AnnouncementTemplate = "AnnouncementTemplate";
        public const string AnnouncementSubject = "لديك إعلان جديد";

        public const string LoginOtpTemplate = "LoginOtpTemplate";
        public const string ResendOtpTemplate = "ResendOtpTemplate";

        public const string ApproveOrderTemplate = "ApproveOrderTemplate";
        public const string CourseLink = "https://students.waey.online/dashboard/courses";
        public const string OrderApprovalSubject = "تمت الموافقة على طلبك";

        public const string RequestFriendTemplate = "RequestFriendTemplate";
        public const string RequestFriendUrl = "https://students.waey.online/dashboard/friends";
        public const string RequestFriendSubject = "لديك طلب صداقة جديد";


        public const string AcceptRequestTemplate = "AcceptRequestTemplate";
        public const string AcceptRequestSubject = "تم قبول طلب الصداقة الخاص بك";

        public const string RejectRequestTemplate = "RejectRequestTemplate";
        public const string RejectRequestSubject = "تم رفض طلب الصداقة الخاص بك";

        public const string NewLessonNotificationTemplate = "NewLessonNotification";
        public const string NewAssignmentNotificationTemplate = "NewAssignmentNotification";
        public const string NewQuizNotificationTemplate = "NewQuizNotification";
        public const string QuizDeadlineReminderTemplate = "QuizDeadlineReminder";
        public const string AssignmentDeadlineReminderTemplate = "AssignmentDeadlineReminder";

        public const string SubscriptionExpiredTemplate = "SubscriptionExpiredTemplate";
        public const string SubscriptionExpiringSoonTemplate = "SubscriptionExpiringSoonTemplate";
    }
}