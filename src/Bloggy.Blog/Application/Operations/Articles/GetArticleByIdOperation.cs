using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Core.Utilities.OperationResult;

namespace Bloggy.Blog.Application.Operations.Articles;

public class GetArticleByIdOperation(IRepositoryManager repository) :
    IOperation<GetArticleByIdCommand, ArticleModel>
{
    public async Task<OperationResult<ArticleModel>> ExecuteAsync(
        GetArticleByIdCommand command, CancellationToken? cancellation = null)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(command.ArticleId))
            return OperationResult<ArticleModel>.ValidationFailure(["ArticleId is required."]);

        // Retrieve the article
        var entity = await repository.Articles.GetByIdAsync(command.ArticleId);
        if (entity is null)
            return OperationResult<ArticleModel>.NotFoundFailure("Article not found.");

        var tags = await repository.Tags.GetByIdsAsync(entity.TagIds);
        var model = entity.MapToModelWithTags(tags);

        return OperationResult<ArticleModel>.Success(model);
    }
}

public record GetArticleByIdCommand(string ArticleId) : IOperationCommand;