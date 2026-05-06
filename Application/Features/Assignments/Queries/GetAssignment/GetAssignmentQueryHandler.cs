using Application.Features.ModuleItems.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Assignments.Queries.GetAssignment
{
    internal sealed class GetAssignmentQueryHandler : IRequestHandler<GetAssignmentQuery, OneOf<AssignmentDto, Error>>
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        public GetAssignmentQueryHandler(IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository,
           IModuleRepository moduleRepository, IModuleItemRepository moduleItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _courseRepository = courseRepository;
            _moduleRepository = moduleRepository;
            _moduleItemRepository = moduleItemRepository;
        }
        public async Task<OneOf<AssignmentDto, Error>> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var assignment = await _moduleItemRepository.GetAssignmentAsync(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (assignment is null)
            {
                return ModuleItemErrors.ModuleItemNotFound;
            }
            return assignment;
        }
    }
}
