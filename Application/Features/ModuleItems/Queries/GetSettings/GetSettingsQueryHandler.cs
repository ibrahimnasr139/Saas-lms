using Application.Features.ModuleItems.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ModuleItems.Queries.GetSettings
{
    internal sealed class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, OneOf<SettingsDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        public GetSettingsQueryHandler(IHttpContextAccessor httpContextAccessor, IModuleItemRepository moduleItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
        }
        public async Task<OneOf<SettingsDto, Error>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var settings = await _moduleItemRepository.GetSettingsAsync(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (settings is null)
            {
                return ModuleItemErrors.ModuleItemNotFound;
            }
            return settings;
        }
    }
}
