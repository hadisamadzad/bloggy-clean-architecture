using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Utilities.OperationResult;

namespace Bloggy.Blog.Application.Operations.Articles;

public class UpdateArticleStatusOperation(IRepositoryManager repository) :
    IOperation<UpdateArticleStatusCommand, NoResult>
{
    public async Task<OperationResult<NoResult>> ExecuteAsync(
        UpdateArticleStatusCommand request, CancellationToken? cancellation = null)
    {
        var entity = await repository.Articles.GetByIdAsync(request.ArticleId);
        if (entity is null)
            return OperationResult.ValidationFailure(["Article not found."]);

        // Update timestamps
        if (request.Status == ArticleStatus.Archived)
            entity.ArchivedAt = DateTime.UtcNow;

        if (request.Status == ArticleStatus.Published && entity.Status == ArticleStatus.Draft)
            entity.PublishedAt = DateTime.UtcNow;

        entity.Status = request.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Articles.UpdateAsync(entity);

        return OperationResult.Success();
    }
}

public record UpdateArticleStatusCommand(string ArticleId, ArticleStatus Status) : IOperationCommand;