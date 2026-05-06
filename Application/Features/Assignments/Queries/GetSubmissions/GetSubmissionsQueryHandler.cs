using Application.Features.Assignments.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Assignments.Queries.GetSubmissions
{
    internal sealed class GetSubmissionsQueryHandler : IRequestHandler<GetSubmissionsQuery, OneOf<IEnumerable<StudentSubmissionDto>, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IModuleItemRepository _moduleItemRepository;
        public GetSubmissionsQueryHandler(IHttpContextAccessor httpContextAccessor, IAssignmentRepository assignmentRepository,
            IModuleItemRepository moduleItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _assignmentRepository = assignmentRepository;
            _moduleItemRepository = moduleItemRepository;
        }
        public async Task<OneOf<IEnumerable<StudentSubmissionDto>, Error>> Handle(GetSubmissionsQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var assignment = await _moduleItemRepository.GetAssignmentAsync(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (assignment is null)
                return ModuleItemErrors.ModuleItemNotFound;
            return await _assignmentRepository.GetSubmissionsAsync(request.CourseId, request.ItemId, cancellationToken);
        }
    }
}
