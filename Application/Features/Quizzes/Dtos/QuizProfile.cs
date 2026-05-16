using Application.Features.Quizzes.Dtos;
using Domain.Enums;

public sealed class QuizProfile : Profile
{
    public QuizProfile()
    {
        CreateMap<AiResponse, QuizQuestion>()
            .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src));

        CreateMap<AiResponse, Question>()
            .ForMember(dest => dest.QuestionTitle, opt => opt.MapFrom(src => src.Question))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ParseQuestionType(src.Type)))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => ParseDifficulty(src.Difficulty)));
    }
    private static QuestionType ParseQuestionType(string type) => type.ToLower() switch
    {
        "mcq" => QuestionType.Mcq,
        "true_false" => QuestionType.true_false,
        "short_answer" => QuestionType.short_answer,
        _ => QuestionType.Mcq
    };
    private static Difficulty ParseDifficulty(string difficulty) => difficulty.ToLower() switch
    {
        "easy" => Difficulty.Easy,
        "medium" => Difficulty.Medium,
        "hard" => Difficulty.Hard,
        _ => Difficulty.Medium
    };
}