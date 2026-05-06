using System.Net;

namespace Domain.Errors
{
    public static class MemberErrors
    {
        public static Error NotAllowed =>
            new("Member.NotAllowed", "ليس لديك صلاحية الوصول إلى هذا المورد", HttpStatusCode.Forbidden);
    }
}
