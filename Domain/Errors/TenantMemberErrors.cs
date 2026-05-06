using System.Net;

namespace Domain.Errors
{
    public static class TenantMemberErrors
    {
        public static Error MemberNotFound =>
            new Error("Member.NotFound", "العضو غير موجود", HttpStatusCode.NotFound);

        public static Error CannotRemoveOwner =>
            new Error("Member.CannotRemoveOwner", "لا يمكن ازالة مالك المةصه", HttpStatusCode.Forbidden);

        public static Error CannotRemoveSelf =>
            new Error("Member.CannotRemoveSelf", "لا يمكنك ازالة نفسك", HttpStatusCode.Forbidden);

        public static Error CannotChangeOwnerRole =>
            new Error("Member.CannotChangeOwnerRole", "لا يمكن تغيير دور مالك المنصة", HttpStatusCode.Forbidden);

        public static Error CannotChangeOwnRole =>
            new Error("Member.CannotChangeOwnRole", "لا يمكنك تغيير دورك بنفسك", HttpStatusCode.Forbidden);

        public static Error CannotDeleteOwnerRole =>
            new Error("Member.CannotDeleteOwnerRole", "لا يمكن حذف دور مالك المنصة", HttpStatusCode.Forbidden);

    }
}