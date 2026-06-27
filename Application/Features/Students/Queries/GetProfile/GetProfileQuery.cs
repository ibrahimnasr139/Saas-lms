using Application.Features.Students.Dtos;

namespace Application.Features.Students.Queries.GetProfile
{
    public sealed record GetProfileQuery : IRequest<OneOf<StudentUserProfileDto, Error>>;
}