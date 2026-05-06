using Application.Features.ModuleItems.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ModuleItems.Queries.GetItem
{
    internal sealed class GetItemQueryHandler : IRequestHandler<GetItemQuery, OneOf<ItemDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly IMapper _mapper;
        public GetItemQueryHandler(IHttpContextAccessor httpContextAccessor, IMapper mapper, IModuleItemRepository moduleItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _moduleItemRepository = moduleItemRepository;
        }
        public async Task<OneOf<ItemDto, Error>> Handle(GetItemQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var moduleItem = await _moduleItemRepository.GetAsync(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (moduleItem is null)
            {
                return ModuleItemErrors.ModuleItemNotFound;
            }
            return _mapper.Map<ItemDto>(moduleItem);
        }
    }
}
