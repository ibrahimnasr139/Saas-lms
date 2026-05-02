using Application.Features.Assignments.Commands.UpdateAssignment;
using Application.Features.Lessons.Commands.UpdateLesson;
using Application.Features.ModuleItems.Commands.CreateModuleItem;
using Application.Features.ModuleItems.Commands.UpdateSettings;
using Application.Features.Quizzes.Commands.UpdateQuiz;
namespace Application.Features.ModuleItems.Dtos
{
    public class ModuleItemProfile : Profile
    {
        public ModuleItemProfile()
        {
            CreateMap<CreateModuleItemCommand, ModuleItem>();

            CreateMap<ConditionDto, ModuleItemCondition>()
                .ForMember(dest => dest.RequiredModuleItemId, opt => opt.MapFrom(src => src.RequiredItemId))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<CreateModuleItemCommand, Lesson>();

            CreateMap<CreateModuleItemCommand, Assignment>()
               .ForMember(dest => dest.Marks, opt => opt.MapFrom(src => src.TotalMarks));

            CreateMap<CreateModuleItemCommand, Quiz>();

            CreateMap<UpdateLessonCommand, Lesson>()
                .ForPath(dest => dest.ModuleItem.Title, opt => opt.MapFrom(src => src.Title))
                .ForPath(dest => dest.ModuleItem.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<UpdateAssignmentCommand, Assignment>()
                .ForPath(dest => dest.ModuleItem.Title, opt => opt.MapFrom(src => src.Title))
                .ForPath(dest => dest.ModuleItem.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Marks, opt => opt.MapFrom(src => src.TotalMarks));

            CreateMap<ModuleItem, ItemDto>();

            CreateMap<Assignment, AssignmentDto>()
                .ForMember(dest => dest.TotalMarks, opt => opt.MapFrom(src => src.Marks))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ModuleItemId));

            CreateMap<ModuleItem, SettingsDto>();

            CreateMap<ModuleItemCondition, ConditionDto>()
                .ForMember(dest => dest.RequiredItemId, opt => opt.MapFrom(src => src.RequiredModuleItemId));

            CreateMap<ModuleItem, AllItemsDto>();

            CreateMap<UpdateSettingsCommand, ModuleItem>()
                .ForMember(dest => dest.Conditions, opt => opt.Ignore());

            CreateMap<UpdateQuizCommand, Quiz>()
                .ForMember(dest => dest.Questions, opt => opt.Ignore());

            CreateMap<Quiz, QuizDto>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions.OrderBy(x => x.Order)));

            CreateMap<QuizQuestion, QuizQuestionDto>()
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question.QuestionTitle))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Question.Type))
                .ForMember(dest => dest.CorrectAnswer, opt => opt.MapFrom(src => src.Question.CorrectAnswer))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Question.Options))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Question.QuestionCategoryId))
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.Question.Difficulty));
        }
    }
}