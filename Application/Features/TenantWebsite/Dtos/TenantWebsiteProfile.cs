using Application.Features.Public.Dtos;
using Domain.Enums;

namespace Application.Features.TenantWebsite.Dtos
{
    public sealed class TenantWebsiteProfile : Profile
    {
        public TenantWebsiteProfile()
        {
            CreateMap<TenantPage, TenantPagesDto>();

            CreateMap<BlockType, BlockTypeDto>();

            CreateMap<PageBlock, TenantBlockTypeDto>()
                .ForMember(dest => dest.BlockType, opt => opt.MapFrom(src => src.BlockTypeId))
                .ForMember(dest => dest.Schema, opt => opt.MapFrom(src => src.BlockType.Schema.RootElement))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.BlockType.Description))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.BlockType.DisplayName));

            CreateMap<TenantMember, InstructorDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture));

            CreateMap<Course, TenantCourseDto>()
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => src.CourseStatus == CourseStatus.Published))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src => src.Subject.Label))
                .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade.Label))
                .ForMember(dest => dest.StudentsCount, opt => opt.MapFrom(src => src.Enrollments.Count))
                .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.Tenant.TenantMembers.FirstOrDefault()));

            CreateMap<TenantPage, TenantNavigationLinkDto>();
        }
    }
}