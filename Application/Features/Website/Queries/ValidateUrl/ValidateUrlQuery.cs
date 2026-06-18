using Application.Features.Website.Dtos;

namespace Application.Features.Website.Queries.ValidateUrl
{
    public sealed record ValidateUrlQuery(string Url) : IRequest<ValidateUrlDto>;
}
