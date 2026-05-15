using Application.Features.Tenants.Dtos;

namespace Application.Features.Tenants.Commands.GenerateDescription
{
    public sealed record GenerateDescriptionCommand(string Title, DescriptionType Context, MetadataDto Metadata) 
        : IRequest<OneOf<GenerateDescriptionResponse, Error>>;
}