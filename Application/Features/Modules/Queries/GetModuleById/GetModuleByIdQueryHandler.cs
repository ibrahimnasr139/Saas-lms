using Application.Features.Modules.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Modules.Queries.GetModuleById
{
    internal sealed class GetModuleByIdQueryHandler : IRequestHandler<GetModuleByIdQuery, OneOf<ModuleDto, Error>>
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetModuleByIdQueryHandler(IModuleRepository moduleRepository, IHttpContextAccessor httpContextAccessor)
        {
            _moduleRepository = moduleRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<OneOf<ModuleDto, Error>> Handle(GetModuleByIdQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var module = await _moduleRepository.GetModuleWithItemsAsync(request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (module is null)
                return ModuleErrors.ModuleNotFound;
            return module;
        }
    }
}
