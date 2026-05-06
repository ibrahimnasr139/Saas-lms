using Application.Features.TenantStudents.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TenantStudents.Queries.GetStudentStatistics
{
    internal sealed class GetStudentStatisticsQueryHandler : IRequestHandler<GetStudentStatisticsQuery, StudentStatisticsDto>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetStudentStatisticsQueryHandler(IStudentRepository studentRepository, IHttpContextAccessor httpContextAccessor)
        {
            _studentRepository = studentRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<StudentStatisticsDto> Handle(GetStudentStatisticsQuery request, CancellationToken cancellationToken)
        {
            var subDomain = _httpContextAccessor?.HttpContext?.Request.Cookies[AuthConstants.SubDomain];
            return await _studentRepository.GetStudentStatisticsAsync(subDomain!, cancellationToken);
        }
    }
}