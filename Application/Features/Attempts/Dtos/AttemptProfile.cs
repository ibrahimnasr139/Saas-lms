namespace Application.Features.Attempts.Dtos
{
    public sealed class AttemptProfile : Profile
    {
        public AttemptProfile()
        {
            CreateMap<QuizAttempt, SummaryDto>()
                .ForMember(dest => dest.Correct, opt => opt.MapFrom(src => src.Answers.Count(ans => ans.IsCorrect)))
                .ForMember(dest => dest.Wrong, opt => opt.MapFrom(src => src.Answers.Count(ans => !ans.IsCorrect && ans.StudentAnswer != null)))
                .ForMember(dest => dest.Skipped, opt => opt.MapFrom(src => src.Answers.Count(ans => ans.StudentAnswer == null)));

            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture));

            CreateMap<QuizAttempt, AttemptResponse>()
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Answers.Count))
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Answers))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src));

            CreateMap<Answer, QuestionAttempt>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuizQuestionId))
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.QuizQuestion.Question.QuestionTitle))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.QuizQuestion.Question.Options))
                .ForMember(dest => dest.CorrectAnswer, opt => opt.MapFrom(src => src.QuizQuestion.Question.CorrectAnswer))
                .ForMember(dest => dest.Explanation, opt => opt.MapFrom(src => src.QuizQuestion.Question.Explanation))
                .ForMember(dest => dest.Marks, opt => opt.MapFrom(src => src.QuizQuestion.Marks))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.QuizQuestion.Question.Type))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.QuizQuestion.Order))
                .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => new AnswerDto
                {
                    Value = src.StudentAnswer,
                    IsCorrect = src.IsCorrect,
                    AutoScore = src.AutoScore,
                    TeacherScore = src.TeacherScore,
                    Feedback = src.Feedback,
                }));
        }
    }
}
