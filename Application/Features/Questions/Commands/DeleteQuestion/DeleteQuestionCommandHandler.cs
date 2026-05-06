using Microsoft.AspNetCore.Http;

namespace Application.Features.Questions.Commands.DeleteQuestion
{
    internal sealed class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, OneOf<bool, Error>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteQuestionCommandHandler(IQuestionRepository questionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _questionRepository = questionRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<bool, Error>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var question = await _questionRepository.GetQuestion(request.QuestionId, subDomain!, cancellationToken);
            if (question == null)
            {
                return QuestionErrors.QuestionNotFound;
            }
            await _questionRepository.RemoveAsync(question, cancellationToken);
            return true;
        }
    }
}
