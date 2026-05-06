using Application.Features.Students.Dtos;

namespace Application.Features.Students.Queries.GetAvailableSubjects
{
    internal sealed class GetAvailableSubjectsQueryHandler : IRequestHandler<GetAvailableSubjectsQuery, List<AvailableSubjectDto>>
    {
        private readonly IStudentRepository _studentRepository;

        public GetAvailableSubjectsQueryHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        public async Task<List<AvailableSubjectDto>> Handle(GetAvailableSubjectsQuery request, CancellationToken cancellationToken)
        {
            return await _studentRepository.GetAvailableSubjectsAsync(cancellationToken);
        }
    }
}