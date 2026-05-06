using FluentValidation;

namespace Application.Features.Modules.Commands.CreateModule
{
    public sealed class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
    {
        public CreateModuleCommandValidator()
        {
            RuleFor(x => x.Order)
                .GreaterThan(0)
                .WithMessage("Order must be greater than 0");
        }
    }
}
