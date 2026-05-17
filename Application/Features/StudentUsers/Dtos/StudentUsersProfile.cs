using Application.Features.StudentUsers.Commands.Onboarding;

namespace Application.Features.StudentUsers.Dtos
{
    public sealed class StudentUsersProfile : Profile
    {
        public StudentUsersProfile()
        {
            CreateMap<ApplicationUser, StudentUserProfileDto>();

            CreateMap<OnboardingCommand, Student>();

            CreateMap<StudentStreak, StudentStreakDto>();

            CreateMap<Student, CurrentStudentDto>()
                .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Gamification, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Streak, opt => opt.MapFrom(src => src.StudentStreak));

            CreateMap<ApplicationUser, ProfileDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.ProfilePicture));

            CreateMap<Student, GamificationDto>()
                .ForMember(dest => dest.Xp, opt => opt.MapFrom(src => src.XP))
                .ForMember(dest => dest.Level, opt => opt.Ignore())
                .ForMember(dest => dest.NextLevelXp, opt => opt.Ignore());

            CreateMap<StudentStreak, StreakDto>();
        }
    }
}