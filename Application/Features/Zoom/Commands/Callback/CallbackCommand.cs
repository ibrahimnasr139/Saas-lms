namespace Application.Features.Zoom.Commands.Callback
{
    public sealed record CallbackCommand(string code, string state) : IRequest<string>;
}
