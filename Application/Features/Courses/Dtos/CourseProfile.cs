using Application.Features.Courses.Commands.CreateCourse;
using Application.Features.Courses.Commands.UpdateCourse;

namespace Application.Features.Courses.Dtos
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CreateCourseCommand, Course>()
                .ForMember(dest => dest.ThumbnailUrl, src => src.MapFrom(c => c.Thumbnail))
                .ForMember(dest => dest.VideoUrl, src => src.MapFrom(c => c.Video))
                .ForMember(dest => dest.CourseStatus, src => src.MapFrom(c => c.Status));

            CreateMap<UpdateCourseCommand, Course>()
                .ForMember(dest => dest.PricingType, src => src.MapFrom(c => c.PriceType))
                .ForMember(dest => dest.ThumbnailUrl, src => src.MapFrom(c => c.Thumbnail))
                .ForMember(dest => dest.VideoUrl, src => src.MapFrom(c => c.Video))
                .ForMember(dest => dest.CourseStatus, src => src.MapFrom(c => c.Status));

            CreateMap<Course, LookupDto>();

            CreateMap<Course, SingleCourseDto>()
                .ForMember(dest => dest.Thumbnail, src => src.MapFrom(c => c.ThumbnailUrl))
                .ForMember(dest => dest.Video, src => src.MapFrom(c => c.VideoUrl))
                .ForMember(dest => dest.Status, src => src.MapFrom(c => c.CourseStatus));

            CreateMap<Course, CourseStatisticsDto>()
                .ForMember(dest => dest.TotalModules, src => src.MapFrom(c => c.Modules.Count))
                .ForMember(dest => dest.TotalLessons, src => src.MapFrom(c => c.Lessons.Count))
                .ForMember(dest => dest.TotalAssignments, src => src.MapFrom(c => c.Assignments.Count))
                .ForMember(dest => dest.TotalQuizzes, src => src.MapFrom(c => c.Quizzes.Count))
                .ForMember(dest => dest.TotalStudents, src => src.MapFrom(c => c.Enrollments.Count));
        }
    }
}