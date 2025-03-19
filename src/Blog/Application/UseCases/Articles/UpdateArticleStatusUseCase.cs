using Blog.Application.Constants;
using Blog.Application.Interfaces;
using Blog.Application.Types.Entities;
using Blog.Application.Types.Models.Articles;
using Common.Utilities.OperationResult;
using MediatR;

namespace Blog.Application.UseCases.Articles;

// Handler
public class UpdateArticleStatusHandler(IRepositoryManager repository) :
    IRequestHandler<UpdateArticleStatusCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateArticleStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.Articles.GetByIdAsync(request.ArticleId);
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.ArticleNotFound);

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