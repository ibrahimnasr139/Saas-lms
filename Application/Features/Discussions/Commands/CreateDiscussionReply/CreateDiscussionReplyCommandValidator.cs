using FluentValidation;

namespace Application.Features.Discussions.Commands.CreateDiscussionReply
{
    public sealed class CreateDiscussionReplyCommandValidator : AbstractValidator<CreateDiscussionReplyCommand>
    {
        public CreateDiscussionReplyCommandValidator()
        {
            RuleFor(dr => dr.Content)
                .NotEmpty().WithMessage("Content is required")
                .MaximumLength(3000).WithMessage("Content must not exceed 3000 characters");
        }
    }
}