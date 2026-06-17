using Application.Features.Tenants.Dtos;

namespace Application.Features.Dashboards.Dtos
{
    public sealed class DashboardsProfile : Profile
    {
        public DashboardsProfile()
        {
            CreateMap<LiveSession, UpcomingSessionsDto>()
                .ForMember(dest => dest.SessionTitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.ActualStartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.ActualEndTime))
                .ForMember(dest => dest.StudentsEnrolled, opt => opt.MapFrom(src => src.Course.Enrollments.Count()))
                .ForMember(dest => dest.Course, opt => opt.MapFrom(src => src.Course));

            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title));
        }
    }
}