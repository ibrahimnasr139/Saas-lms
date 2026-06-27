using Application.Features.Students.Dtos;

namespace Application.Features.Students.Queries.GetProfileDetails
{
    public sealed record GetProfileDetailsQuery : IRequest<OneOf<ProfileDetailsDto, Error>>;
}