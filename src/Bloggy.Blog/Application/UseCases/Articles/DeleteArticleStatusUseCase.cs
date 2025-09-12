using Bloggy.Blog.Application.Constants;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Core.Utilities.OperationResult;
using MediatR;

namespace Bloggy.Blog.Application.UseCases.Articles;

// Handler
public class DeleteArticleHandler(IRepositoryManager repository) :
    IRequestHandler<DeleteArticleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.Articles.GetByIdAsync(request.ArticleId);
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.ArticleNotFound);

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;

        _ = await repository.Articles.UpdateAsync(entity);

        return OperationResult.Success(entity.MapToModel());
    }
}

// Model
public record DeleteArticleCommand(string ArticleId) : IRequest<OperationResult>;