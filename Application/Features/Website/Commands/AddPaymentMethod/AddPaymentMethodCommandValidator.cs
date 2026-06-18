using Domain.Enums;
using FluentValidation;

namespace Application.Features.Website.Commands.AddPaymentMethod
{
    public sealed class AddPaymentMethodCommandValidator : AbstractValidator<AddPaymentMethodCommand>
    {
        private static readonly Dictionary<PaymentMethodType, string[]> RequiredKeys = new()
        {
            [PaymentMethodType.Instapay] = ["phoneNumber", "accountName"],
            [PaymentMethodType.VodafoneCash] = ["phoneNumber"],
            [PaymentMethodType.OrangeCash] = ["phoneNumber"],
            [PaymentMethodType.Fawry] = ["fawryCode"],
            [PaymentMethodType.BankTransfer] = ["bankName", "accountHolderName", "accountNumber"]
        };

        private static readonly HashSet<PaymentMethodType> PhoneNumberTypes =
        [
            PaymentMethodType.Instapay,
            PaymentMethodType.VodafoneCash,
            PaymentMethodType.OrangeCash
        ];

        private const string EgyptianPhonePattern = @"^(?:\+201[0125]\d{8}|01[0125]\d{8})$";

        public AddPaymentMethodCommandValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid payment method type.");

            RuleFor(x => x.Details)
                .NotNull()
                .WithMessage("Details are required.")
                .Must(d => d.Count > 0)
                .WithMessage("Details cannot be empty.");

            RuleFor(x => x)
                .Must(HaveRequiredKeys)
                .WithMessage(x =>
                {
                    var required = RequiredKeys.TryGetValue(x.Type, out var keys)
                        ? string.Join(", ", keys)
                        : "unknown";
                    return $"Missing required fields for {x.Type}: {required}.";
                })
                .When(x => x.Details is { Count: > 0 });

            RuleFor(x => x)
                .Must(HaveNoEmptyValues)
                .WithMessage("Details values cannot be empty or whitespace.")
                .When(x => x.Details is { Count: > 0 });

            RuleFor(x => x)
                .Must(HaveValidPhoneNumber)
                .WithMessage("Phone number must be a valid Egyptian number (e.g. 01012345678 or +201012345678).")
                .When(x => PhoneNumberTypes.Contains(x.Type)
                    && x.Details is { Count: > 0 }
                    && x.Details.ContainsKey("phoneNumber"));
        }

        private static bool HaveRequiredKeys(AddPaymentMethodCommand command)
        {
            if (!RequiredKeys.TryGetValue(command.Type, out var required))
                return true;

            return required.All(key =>
                command.Details.ContainsKey(key) &&
                !string.IsNullOrWhiteSpace(command.Details[key].GetString()));
        }

        private static bool HaveNoEmptyValues(AddPaymentMethodCommand command)
        {
            return command.Details.Values.All(v =>
                !string.IsNullOrWhiteSpace(v.GetString()));
        }

        private static bool HaveValidPhoneNumber(AddPaymentMethodCommand command)
        {
            var phone = command.Details["phoneNumber"].GetString();
            return !string.IsNullOrWhiteSpace(phone)
                && System.Text.RegularExpressions.Regex.IsMatch(phone, EgyptianPhonePattern);
        }
    }
}