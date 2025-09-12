using Bloggy.Blog.Application.Constants;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Core.Utilities.OperationResult;
using MediatR;

namespace Bloggy.Blog.Application.UseCases.Articles;

// Handler
public class UpdateArticleStatusHandler(IRepositoryManager repository) :
    IRequestHandler<UpdateArticleStatusCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateArticleStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.Articles.GetByIdAsync(request.ArticleId);
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.ArticleNotFound);

        // Update timestamps
        if (request.Status == ArticleStatus.Archived)
            entity.ArchivedAt = DateTime.UtcNow;

        if (request.Status == ArticleStatus.Published && entity.Status == ArticleStatus.Draft)
            entity.PublishedAt = DateTime.UtcNow;

        entity.Status = request.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Articles.UpdateAsync(entity);

        return OperationResult.Success(entity.MapToModel());
    }
}

// Model
public record UpdateArticleStatusCommand(
    string ArticleId,
    ArticleStatus Status
) : IRequest<OperationResult>;