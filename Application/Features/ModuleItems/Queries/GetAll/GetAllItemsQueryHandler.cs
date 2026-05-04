using Application.Features.ModuleItems.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ModuleItems.Queries.GetAll
{
    internal sealed class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, OneOf<IEnumerable<AllItemsDto>, Error>>
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        public GetAllItemsQueryHandler(IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository,
           IModuleRepository moduleRepository, IModuleItemRepository moduleItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _courseRepository = courseRepository;
            _moduleRepository = moduleRepository;
            _moduleItemRepository = moduleItemRepository;
        }
        public async Task<OneOf<IEnumerable<AllItemsDto>, Error>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var course = await _courseRepository.GetCourseByIdAsync(request.CourseId, subdomain!, cancellationToken);
            if (course is null)
                return CourseErrors.CourseNotFound;

            var module = await _moduleRepository.GetModuleByIdAsync(request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (module is null)
                return ModuleErrors.ModuleNotFound;

            return await _moduleItemRepository.GetAllItemsAsync(request.ItemId, request.ModuleId, request.Type, request.CourseId, cancellationToken);
        }
    }
}
