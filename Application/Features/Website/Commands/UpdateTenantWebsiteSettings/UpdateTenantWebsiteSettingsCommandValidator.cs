using FluentValidation;

namespace Application.Features.Website.Commands.UpdateTenantWebsiteSettings
{
    public sealed class UpdateTenantWebsiteSettingsCommandValidator : AbstractValidator<UpdateTenantWebsiteSettingsCommand>
    {
        public UpdateTenantWebsiteSettingsCommandValidator()
        {
            RuleFor(x => x)
                .Must(x => x.General != null || x.Email != null || x.Notifications != null || x.Appearance != null)
                .WithMessage("At least one settings section must be provided.");

            When(x => x.General != null, () =>
            {
                RuleFor(x => x.General!.PlatformName)
                    .NotEmpty().WithMessage("Platform name is required.")
                    .MaximumLength(100).WithMessage("Platform name must not exceed 100 characters.");

                RuleFor(x => x.General!.HomepageId)
                    .GreaterThan(0).WithMessage("Homepage ID must be a valid ID.");
            });

            When(x => x.Email != null, () =>
            {
                RuleFor(x => x.Email!.SenderName)
                    .NotEmpty().WithMessage("Sender name is required.")
                    .MaximumLength(100).WithMessage("Sender name must not exceed 100 characters.");

                RuleFor(x => x.Email!.SenderEmail)
                    .NotEmpty().WithMessage("Sender email is required.")
                    .EmailAddress().WithMessage("Sender email is not valid.")
                    .MaximumLength(256).WithMessage("Sender email must not exceed 256 characters.");

                RuleFor(x => x.Email!.ReplyToEmail)
                    .NotEmpty().WithMessage("Reply-to email is required.")
                    .EmailAddress().WithMessage("Reply-to email is not valid.")
                    .MaximumLength(256).WithMessage("Reply-to email must not exceed 256 characters.");
            });

            When(x => x.Notifications != null, () =>
            {
                RuleFor(x => x.Notifications!.Email)
                    .NotNull().WithMessage("Notification email settings are required.");
            });

            When(x => x.Appearance != null, () =>
            {
                RuleFor(x => x.Appearance!.FontFamily)
                    .NotEmpty().WithMessage("Font family is required.")
                    .MaximumLength(100).WithMessage("Font family must not exceed 100 characters.");

                RuleFor(x => x.Appearance!.Direction)
                    .IsInEnum().WithMessage("Direction must be either RTL or LTR.");

                RuleFor(x => x.Appearance!.Logo)
                    .MaximumLength(2048).WithMessage("Logo URL must not exceed 2048 characters.")
                    .When(x => x.Appearance!.Logo != null);

                RuleFor(x => x.Appearance!.Favicon)
                    .MaximumLength(2048).WithMessage("Favicon URL must not exceed 2048 characters.")
                    .When(x => x.Appearance!.Favicon != null);
            });
        }
    }
}