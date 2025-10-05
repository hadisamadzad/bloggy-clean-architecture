using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Blog.Application.Types.Models.Settings;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Core.Utilities.Pagination;

namespace Bloggy.Blog.Application.Operations.Articles;

public class GetArticlesByFilterOperation(IRepositoryManager repository) :
    IOperation<GetArticlesByFilterCommand, PaginatedList<ArticleModel>>
{
    public async Task<OperationResult<PaginatedList<ArticleModel>>> ExecuteAsync(
        GetArticlesByFilterCommand command, CancellationToken? cancellation = null)
    {
        if (command.Filter is null)
            command = command with { Filter = new() { HasPagination = true } };

        // Retrieve the articles
        var entities = await repository.Articles.GetByFilterAsync(command.Filter);
        var totalCount = await repository.Articles.CountByFilterAsync(command.Filter);
        entities ??= [];

        var result = new PaginatedList<ArticleModel>
        {
            Page = command.Filter.Page,
            PageSize = command.Filter.PageSize,
            TotalCount = totalCount,
            Results = [.. entities.MapToModels()]
        };

        return OperationResult<PaginatedList<ArticleModel>>.Success(result);
    }
}

public record GetArticlesByFilterCommand(ArticleFilter Filter) : IOperationCommand;