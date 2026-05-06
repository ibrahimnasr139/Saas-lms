using FluentValidation;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Application.Features.TenantPaymentMethods.Commands.UpdatePaymentMethod
{
    public sealed class UpdatePaymentMethodCommandValidator : AbstractValidator<UpdatePaymentMethodCommand>
    {
        private const string EgyptianPhonePattern = @"^(?:\+201[0125]\d{8}|01[0125]\d{8})$";

        public UpdatePaymentMethodCommandValidator()
        {
            RuleFor(x => x.Details)
                .NotNull()
                .WithMessage("Details are required.")
                .Must(d => d.Count > 0)
                .WithMessage("Details cannot be empty.");

            RuleFor(x => x.Details)
                .Must(HaveNoEmptyValues)
                .WithMessage("Details values cannot be empty or whitespace.")
                .When(x => x.Details is { Count: > 0 });

            RuleFor(x => x.Details)
                .Must(HaveValidPhoneNumber)
                .WithMessage("Phone number must be a valid Egyptian number (e.g. 01012345678 or +201012345678).")
                .When(x => x.Details is { Count: > 0 } && x.Details.ContainsKey("phoneNumber"));
        }

        private static bool HaveNoEmptyValues(Dictionary<string, JsonElement> details)
        {
            return details.Values.All(v => !string.IsNullOrWhiteSpace(v.GetString()));
        }

        private static bool HaveValidPhoneNumber(Dictionary<string, JsonElement> details)
        {
            var phone = details["phoneNumber"].GetString();
            return !string.IsNullOrWhiteSpace(phone)
                && Regex.IsMatch(phone, EgyptianPhonePattern);
        }
    }
}