namespace Application.Features.Attempts.Dtos
{
    public sealed class AttemptProfile : Profile
    {
        public AttemptProfile()
        {
            CreateMap<QuizAttempt, SummaryDto>()
                .ForMember(dest => dest.Correct, src => src.MapFrom(a => a.Answers.Count(ans => ans.IsCorrect)))
                .ForMember(dest => dest.Wrong, src => src.MapFrom(a => a.Answers.Count(ans => !ans.IsCorrect && ans.StudentAnswer != null)))
                .ForMember(dest => dest.Skipped, src => src.MapFrom(a => a.Answers.Count(ans => ans.StudentAnswer == null)));
            
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.Id, src => src.MapFrom(s => s.Id))
                .ForMember(dest => dest.Name, src => src.MapFrom(s => s.User.FirstName + " " + s.User.LastName))
                .ForMember(dest => dest.ProfilePicture, src => src.MapFrom(s => s.User.ProfilePicture));
            
            CreateMap<QuizAttempt, AttemptResponse>()
                .ForMember(dest => dest.QuestionCount, src => src.MapFrom(a => a.Answers.Count));
            
            CreateMap<Answer, QuestionAttempt>()
                .ForMember(dest => dest.QuestionId, src => src.MapFrom(a => a.QuizQuestionId))
                .ForMember(dest => dest.Question, src => src.MapFrom(a => a.QuizQuestion.Question.QuestionTitle))
                .ForMember(dest => dest.Options, src => src.MapFrom(a => a.QuizQuestion.Question.Options))
                .ForMember(dest => dest.CorrectAnswer, src => src.MapFrom(a => a.QuizQuestion.Question.CorrectAnswer))
                .ForMember(dest => dest.Explanation, src => src.MapFrom(a => a.QuizQuestion.Question.Explanation))
                .ForMember(dest => dest.Marks, src => src.MapFrom(a => a.QuizQuestion.Marks))
                .ForMember(dest => dest.Type, src => src.MapFrom(a => a.QuizQuestion.Question.Type))
                .ForMember(dest => dest.Order, src => src.MapFrom(a => a.QuizQuestion.Order))
                .ForMember(dest => dest.Answer, src => src.MapFrom(a => new AnswerDto
                {
                    Value = a.StudentAnswer,
                    IsCorrect = a.IsCorrect,
                    AutoScore = a.AutoScore,
                    TeacherScore = a.TeacherScore,
                    Feedback = a.Feedback,
                }));
        }
    }
}
