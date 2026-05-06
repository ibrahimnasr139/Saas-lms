using System.Net;

namespace Domain.Errors
{
    public static class TenantErrors
    {
        public static Error SubDomainAlreadyExists =>
            new("Tenant.SubDomainAlreadyExists", "هذا الدومين مستخدم بالفعل", HttpStatusCode.Conflict);
        public static Error FeatureUsageEnded =>
            new("Tenant.FeatureUsageEnded", "لقد تجاوزت الحد المسموح به لهذه الميزة، يرجى ترقية خطتك لاستخدام المزيد", HttpStatusCode.Forbidden);
        public static Error NotSubscribed =>
            new("Tenant.NotSubscribed", "يجب أن يكون لديك اشتراك نشط للوصول إلى هذا المورد", HttpStatusCode.Forbidden);
    }
}