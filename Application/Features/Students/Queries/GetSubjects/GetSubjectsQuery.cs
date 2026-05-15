using Application.Features.Students.Dtos;

namespace Application.Features.Students.Queries.GetSubjects
{
    public sealed record GetSubjectsQuery : IRequest<OneOf<List<SubjectDto>, Error>>;
}