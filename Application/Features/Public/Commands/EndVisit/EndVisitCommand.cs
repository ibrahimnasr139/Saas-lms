namespace Application.Features.Public.Commands.EndVisit
{
    public sealed record EndVisitCommand(string PageUrl, Guid VisitorId, int DurationSecond) : IRequest<OneOf<bool, Error>>;
}