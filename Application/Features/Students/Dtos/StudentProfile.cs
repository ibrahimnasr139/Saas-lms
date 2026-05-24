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
        }
    }
}