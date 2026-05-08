using System.Net;

namespace Domain.Errors
{
    public static class FlashCardErrors
    {
        public static Error NotFound =>
            new Error("FlashCardDeckNotFound", "مجموعة البطاقات التعليمية غير موجودة", HttpStatusCode.NotFound);
    }
}