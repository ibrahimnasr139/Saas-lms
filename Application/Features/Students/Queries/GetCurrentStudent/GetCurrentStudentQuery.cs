using Application.Features.Students.Dtos;

namespace Application.Features.Students.Queries.GetCurrentStudent
{
    public sealed class GetCurrentStudentQuery : IRequest<OneOf<CurrentStudentDto, Error>>;
}