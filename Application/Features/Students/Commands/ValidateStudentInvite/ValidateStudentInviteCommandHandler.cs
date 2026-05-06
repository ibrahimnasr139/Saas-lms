using Application.Features.Students.Dtos;

namespace Application.Features.Students.Commands.ValidateStudentInvite
{
    internal sealed class ValidateStudentInviteCommandHandler : IRequestHandler<ValidateStudentInviteCommand, OneOf<ValidateStudentInviteDto, Error>>
    {
        private readonly ICourseInviteRepository _courseInviteRepository;

        public ValidateStudentInviteCommandHandler(ICourseInviteRepository courseInviteRepository)
        {
            _courseInviteRepository = courseInviteRepository;
        }
        public async Task<OneOf<ValidateStudentInviteDto, Error>> Handle(ValidateStudentInviteCommand request, CancellationToken cancellationToken)
        {
            var result = await _courseInviteRepository.GetValidateStudentInviteAsync(request.Token, cancellationToken);
            if (result is null)
                return StudentErrors.InvalidToken;
            return result;
        }
    }
}