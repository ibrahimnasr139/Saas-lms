using Domain.Enums;

namespace Application.Features.StudentLessons.Dtos
{
    public sealed class StudentLessonsProfile : Profile
    {
        public StudentLessonsProfile()
        {
            CreateMap<ModuleItem, StudentLessonItemDto>()
                .ForMember(dest => dest.IsCompleted, opt => opt.Ignore())
                .ForMember(dest => dest.Resources, opt => opt.MapFrom(src => src.Lesson!.Resources))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Lesson!.File))
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<Domain.Entites.File, ContentDto>()
                .ForMember(dest => dest.VideoId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VideoUrl, opt => opt.MapFrom(src => src.Url));

            CreateMap<DicussionThread, StudentDiscussionDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies));

            CreateMap<DicussionThreadReply, ReplyDto>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Body))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User));

            CreateMap<ApplicationUser, AuthorDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            CreateMap<LessonView, StudentLessonProgressDto>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.Lesson.CourseId))
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.ModuleItemId))
                .ForMember(dest => dest.VideoId, opt => opt.MapFrom(src => src.Lesson.VideoId))
                .ForMember(dest => dest.DurationSeconds, opt => opt.MapFrom(src =>
                    src.Lesson.File.Metadata != null &&
                    src.Lesson.File.Metadata.ContainsKey("duration")
                        ? int.Parse(src.Lesson.File.Metadata["duration"])
                        : 0
                ))
                .ForMember(dest => dest.CompletionPercentage, opt => opt.MapFrom(src =>
                    src.Lesson.File.Metadata != null &&
                    src.Lesson.File.Metadata.ContainsKey("duration") &&
                    double.Parse(src.Lesson.File.Metadata["duration"]) != 0
                        ? (src.WatchedSeconds / double.Parse(src.Lesson.File.Metadata["duration"])) * 100
                        : 0
                ))
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.Status == ViewStatus.Completed))
                .ForMember(dest => dest.ViewsCount, opt => opt.MapFrom(src => src.ViewCount))
                .ForMember(dest => dest.FirstViewedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.LastViewedAt, opt => opt.MapFrom(src => src.LastWatchedAt));
        }
    }
}