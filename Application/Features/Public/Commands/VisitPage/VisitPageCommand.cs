using Domain.Enums;

namespace Application.Features.Public.Commands.VisitPage
{
    public sealed record VisitPageCommand(string PageUrl, Guid VisitorId, DeviceType DeviceType) : IRequest<OneOf<bool, Error>>;
}