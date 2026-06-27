using Application.Features.Students.Commands.Onboarding;
using Application.Features.Students.Commands.UpdateProfile;

namespace Application.Features.Students.Dtos
{
    public sealed class StudentProfile : Profile
    {
        public StudentProfile()
        {
            CreateMap<CourseInvite, ValidateStudentInviteDto>()
               .ForMember(dest => dest.InviterName, opt => opt.MapFrom(src => src.TenantMember.User.FirstName + " " + src.TenantMember.User.LastName))
               .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.AcceptedAt == null && src.ExpiresAt > DateTime.UtcNow))
               .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
               .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.ExpiresAt <= DateTime.UtcNow));

            CreateMap<AvailableSubject, AvailableSubjectDto>();

            CreateMap<StudentSubject, SubjectDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AvailableSubject.DisplayName))
                .ForMember(dest => dest.Chapters, opt => opt.MapFrom(src => src.AvailableSubject.StudentChapters));

            CreateMap<StudentChapter, ChapterDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.TotalLessons, opt => opt.MapFrom(src =>
                    src.Metadata != null && src.Metadata.ContainsKey("total_lessons")
                        ? int.Parse(src.Metadata["total_lessons"])
                        : 0)
                )
                .ForMember(dest => dest.IsCurrentChapter, opt => opt.Ignore());

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

            CreateMap<StudentStreak, ProfileStreakDto>();

            CreateMap<Student, ProfileDetailsDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.ProfilePicture))
                .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.StudentSubjects))
                .ForMember(dest => dest.Gamification, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Streak, opt => opt.MapFrom(src => src.StudentStreak));

            CreateMap<StudentSubject, StudentSubjectProfileDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AvailableSubjectId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AvailableSubject.DisplayName));

            CreateMap<Student, StudentGamificationDto>()
                .ForMember(dest => dest.Xp, opt => opt.MapFrom(src => src.XP))
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level))
                .ForMember(dest => dest.NextLevelXp, opt => opt.Ignore());
        }
    }
}