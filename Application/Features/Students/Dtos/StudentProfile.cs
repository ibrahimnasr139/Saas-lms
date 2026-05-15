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
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AvailableSubject.DisplayName));
        }
    }
}