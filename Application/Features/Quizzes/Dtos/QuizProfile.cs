namespace Application.Features.Quizzes.Dtos
{
    public sealed class QuizProfile : Profile
    {
        public QuizProfile() 
        {
            CreateMap<AiResponse, QuizQuestion>()
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src));

            CreateMap<AiResponse, Question>()
                .ForMember(dest => dest.QuestionTitle, opt => opt.MapFrom(src => src.Question));
        }
    }
}
