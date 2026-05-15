using Domain.Enums;

namespace Application.Features.TenantStudents.Dtos
{
    public sealed class TenantStudentProfile : Profile
    {
        public TenantStudentProfile()
        {
            CreateMap<Student, StudentsDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture))
                .ForMember(dest => dest.AverageGrades, opt => opt.MapFrom(src => src.StudentGrades.Any() ? (int)Math.Round(src.StudentGrades.Average(sg => sg.Score / sg.TotalMarks * 100)) : 0))
                .ForMember(dest => dest.EnrolledCourses, opt => opt.MapFrom(src => src.Enrollments.Select(e => e.CourseId).ToList()))
                .ForMember(dest => dest.Flags, opt => opt.MapFrom(src => src.StudentSubscriptions));

            CreateMap<ICollection<StudentSubscription>, StudentFlagDto>()
                .ForMember(dest => dest.HasActiveSubscription, opt => opt.MapFrom(src => src.Any(s => s.Status == StudentSubscriptionStatus.Active)))
                .ForMember(dest => dest.HasExpiredSubscription, opt => opt.MapFrom(src => src.Any(s => s.Status == StudentSubscriptionStatus.Expired)))
                .ForMember(dest => dest.HasUnpaidCourses, opt => opt.MapFrom(src => src.Any(s => s.Status == StudentSubscriptionStatus.Cancelled)));

            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture))
                .ForMember(dest => dest.AverageGrades, opt => opt.MapFrom(src => src.StudentGrades.Any() ? (int)Math.Round(src.StudentGrades.Average(sg => sg.Score / sg.TotalMarks * 100)) : 0))
                .ForMember(dest => dest.EnrolledCourses, opt => opt.MapFrom(src => src.Enrollments.Select(e => e.CourseId).ToList()))
                .ForMember(dest => dest.Flags, opt => opt.MapFrom(src => src.StudentSubscriptions))
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Enrollments));


            CreateMap<Enrollment, StudentCourseDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.BillingCycle, opt => opt.MapFrom(src => src.Course.BillingCycle))
                .ForMember(dest => dest.PricingType, opt => opt.MapFrom(src => src.Course.PricingType))
                .ForMember(dest => dest.InvitedBy, opt => opt.MapFrom(src => src.Tenant.TenantMembers.Select(tm => tm.DisplayName).FirstOrDefault()))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Student.StudentSubscriptions.Select(sc => sc.StartDate).FirstOrDefault()))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Student.StudentSubscriptions.Select(sc => sc.EndDate).FirstOrDefault()));
        }
    }
}