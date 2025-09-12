using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Core.Utilities.Pagination;
using MediatR;

namespace Bloggy.Blog.Application.UseCases.Articles;

// Handler
public class GetArticlesByFilterHandler(IRepositoryManager repository) :
    IRequestHandler<GetArticlesByFilterQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetArticlesByFilterQuery request, CancellationToken cancellationToken)
    {
        if (request.Filter is null)
            request = request with { Filter = new() { HasPagination = true } };

        // Retrieve the articles
        var entities = await repository.Articles.GetByFilterAsync(request.Filter);
        var totalCount = await repository.Articles.CountByFilterAsync(request.Filter);
        entities ??= [];

        var result = new PaginatedList<ArticleModel>
        {
            Page = request.Filter.Page,
            PageSize = request.Filter.PageSize,
            TotalCount = totalCount,
            Results = [.. entities.MapToModels()]
        };

        return OperationResult.Success(result);
    }
}

// Model
public record GetArticlesByFilterQuery(ArticleFilter Filter) : IRequest<OperationResult>;