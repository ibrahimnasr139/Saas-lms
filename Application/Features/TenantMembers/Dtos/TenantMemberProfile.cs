namespace Application.Features.TenantMembers.Dtos
{
    public sealed class TenantMemberProfile : Profile
    {
        public TenantMemberProfile()
        {
            CreateMap<TenantMember, CurrentTenantMemberDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.TenantRole.Name))
                .ForMember(dest => dest.HasFullAccess, opt => opt.MapFrom(src => src.TenantRole.HasAllPermissions));


            CreateMap<TenantMember, TenantMembersDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.TenantRole.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture));


            CreateMap<TenantInvite, ValidateTenanInviteDto>()
               .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant.PlatformName))
               .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.TenantRole.Name))
               .ForMember(dest => dest.InviterName, opt => opt.MapFrom(src => src.TenantMember.User.FirstName + " " + src.TenantMember.User.LastName))
               .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.AcceptedAt == null && src.ExpiresAt > DateTime.UtcNow))
               .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.ExpiresAt <= DateTime.UtcNow))
               .ForMember(dest => dest.Subdomain, opt => opt.MapFrom(src => src.Tenant.SubDomain))
               .ForMember(dest => dest.RolePermissions, opt => opt.MapFrom(src => src.TenantRole.RolePermissions.Select(rp => rp.Permission.Name).ToList()));


            CreateMap<TenantMember, MemberProfileDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.ProfilePicture, opt => opt.MapFrom(src => src.User.ProfilePicture))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.TenantRole.Name))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.TenantRole.RolePermissions.Select(rp => rp.Permission.Name).ToList()));
        }
    }
}
