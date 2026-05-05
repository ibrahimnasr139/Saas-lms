using Application.Features.Assignments.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Assignments.Queries.GetPerformance
{
    public sealed class GetPerformanceQueryHandler : IRequestHandler<GetPerformanceQuery, OneOf<PerformanceDto, Error>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IModuleItemRepository _moduleItemRepository;
        private readonly ISubmissionRepository _submissionRepository;
        public GetPerformanceQueryHandler(IHttpContextAccessor httpContextAccessor, ISubmissionRepository submissionRepository, IModuleItemRepository moduleItemRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _moduleItemRepository = moduleItemRepository;
            _submissionRepository = submissionRepository;
        }
        public async Task<OneOf<PerformanceDto, Error>> Handle(GetPerformanceQuery request, CancellationToken cancellationToken)
        {
            var subdomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            var assignment = await _moduleItemRepository.GetAssignmentAsync(request.ItemId, request.ModuleId, request.CourseId, subdomain!, cancellationToken);
            if (assignment is null)
                return ModuleItemErrors.ModuleItemNotFound;

            return new PerformanceDto
            {
                GradesDistribution = await _submissionRepository.GetSubmissionGradeDistributionAsync(request.ItemId, assignment.TotalMarks, cancellationToken),
                SubmissionsOverTime = await _submissionRepository.GetSubmissionsOverTimeAsync(request.ItemId, cancellationToken)
            };
        }
    }
}