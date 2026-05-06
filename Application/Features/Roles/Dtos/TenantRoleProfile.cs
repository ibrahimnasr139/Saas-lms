namespace Application.Features.Roles.Dtos
{
    public sealed class TenantRoleProfile : Profile
    {
        public TenantRoleProfile()
        {
            CreateMap<TenantRole, TenantRolesDto>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
                .ForMember(dest => dest.HasFullAccess, opt => opt.MapFrom(src => src.HasAllPermissions))
                .ForMember(dest => dest.PermissionsCount, opt => opt.MapFrom(src => src.RolePermissions.Count))
                .ForMember(dest => dest.IsSystemRole, opt => opt.MapFrom(src => src.Name == TenantRoleConstants.Owner || src.Name == TenantRoleConstants.Assistant))
                .ForMember(dest => dest.MembersCount, opt => opt.MapFrom(src => src.TenantMembers.Count))
                .ForMember(dest => dest.EnabledPermissions, opt => opt.MapFrom(src =>
                    src.RolePermissions.Select(rp => rp.PermissionId).ToList()));
        }
    }
}
