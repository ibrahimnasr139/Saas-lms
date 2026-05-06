using Domain.Enums;

namespace Application.Features.Questions.Dtos
{
    public sealed class QuestionTypeDto
    {
        public QuestionType Type { get; set; }
        public int Count { get; set; }
    }
}
