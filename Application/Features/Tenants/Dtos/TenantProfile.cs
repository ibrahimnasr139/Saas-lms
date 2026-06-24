using Application.Features.Tenants.Commands.CreateOnboarding;
using System.Globalization;

namespace Application.Features.Tenants.Dtos
{
    public class TenantProfile : Profile
    {
        public TenantProfile()
        {
            CreateMap<LabelValueDto, Subject>();

            CreateMap<LabelValueDto, TeachingLevel>();
            
            CreateMap<LabelValueDto, Grade>();
            
            CreateMap<CreateOnboardingCommand, Tenant>();
            
            CreateMap<CreateOnboardingCommand, ApplicationUser>()
                .ForMember(dest => dest.LastActiveTenantSubDomain, opt => opt.MapFrom(src => src.SubDomain));

            CreateMap<CreateOnboardingCommand, TenantMember>();

            CreateMap<Tenant, LastTenantDto>()
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.Subjects))
                .ForMember(dest => dest.TeachingLevels, opt => opt.MapFrom(src => src.TeachingLevels))
                .ForMember(dest => dest.Grades, opt => opt.MapFrom(src => src.Grades));
            
            CreateMap<Subject, LabelValueIdDto>();
            
            CreateMap<TeachingLevel, LabelValueIdDto>();
            
            CreateMap<Grade, LabelValueIdDto>();

            CreateMap<Domain.Entites.File, DocumentDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => Path.GetExtension(src.Url).TrimStart('.')));

            CreateMap<Domain.Entites.File, ImageDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => Path.GetExtension(src.Url).TrimStart('.')));

            CreateMap<Domain.Entites.File, VideoDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Metadata != null && src.Metadata.ContainsKey("duration") ? double.Parse(src.Metadata["duration"], CultureInfo.InvariantCulture) : 0.0))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => Path.GetExtension(src.Url).TrimStart('.')));


            CreateMap<LiveSession, LiveSessionDto>()
               .ForMember(dest => dest.Course, opt => opt.MapFrom(src => new CourseDto
               {
                   Id = src.Course.Id,
                   Name = src.Course.Title
               }))
               .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.ScheduledAt))
               .ForMember(dest => dest.Teacher, opt => opt.MapFrom(src => src.ZoomIntegration!.ZoomDisplayName))
               .ForMember(dest => dest.Attendance, opt => opt.MapFrom(src => src.Participants.Count))
               .ForMember(dest => dest.Recorded, opt => opt.MapFrom(src => src.RecordingUrl != null))
               .ForMember(dest => dest.JoinUrl, opt => opt.MapFrom(src => src.ZoomJoinUrl))
               .ForMember(dest => dest.TotalStudents, opt => opt.MapFrom(src => src.Course.Enrollments.Count))
               .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => new LiveSessionSettingsDto
               {
                   EnableChat = src.EnableChat,
                   ParticipantVideo = src.ParticipantVideo,
                   WaitingRoom = src.WaitingRoom
               }));
        }
    }
}