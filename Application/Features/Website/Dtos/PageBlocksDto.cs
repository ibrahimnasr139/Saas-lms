namespace Application.Features.Website.Dtos
{
    public sealed record PageBlocksDto(string BlockType, int Order, bool Visible, Dictionary<string, object> Props);
}
