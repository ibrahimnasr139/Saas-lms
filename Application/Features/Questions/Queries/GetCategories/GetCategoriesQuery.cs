using Application.Features.Questions.Dtos;

namespace Application.Features.Questions.Queries.GetCategories
{
    public sealed record GetCategoriesQuery : IRequest<List<QuestionCategoryDto>>;
}
