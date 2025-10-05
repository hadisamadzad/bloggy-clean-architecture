using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Core.Utilities.OperationResult;

namespace Bloggy.Blog.Application.Operations.Articles;

public class GetArticleBySlugOperation(IRepositoryManager repository) :
    IOperation<GetArticleBySlugCommand, ArticleModel>
{
    public async Task<OperationResult<ArticleModel>> ExecuteAsync(
        GetArticleBySlugCommand command, CancellationToken? cancellation = null)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(command.Slug))
            return OperationResult<ArticleModel>.ValidationFailure(["Slug is required."]);

        command = command with { Slug = command.Slug.ToLower() };

        // Retrieve the article
        var entity = await repository.Articles.GetBySlugAsync(command.Slug);
        if (entity is null)
            return OperationResult<ArticleModel>.NotFoundFailure("Article not found.");

        var tags = await repository.Tags.GetByIdsAsync(entity.TagIds);
        var model = entity.MapToModelWithTags(tags);

        return OperationResult<ArticleModel>.Success(model);
    }
}

public record GetArticleBySlugCommand(string Slug) : IOperationCommand;