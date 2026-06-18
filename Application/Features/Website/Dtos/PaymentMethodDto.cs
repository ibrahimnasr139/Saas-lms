using Domain.Enums;
using System.Text.Json;

namespace Application.Features.Website.Dtos
{
    public sealed class PaymentMethodDto
    {
        public int Id { get; set; }
        public PaymentMethodType Type { get; set; }
        public bool IsActive { get; set; }
        public Dictionary<string, JsonElement> Details { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }
}
