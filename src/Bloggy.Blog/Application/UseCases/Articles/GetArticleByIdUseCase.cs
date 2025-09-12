using Bloggy.Blog.Application.Constants;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Core.Utilities.OperationResult;
using MediatR;

namespace Bloggy.Blog.Application.UseCases.Articles;

// Handler
public class GetArticleByIdHandler(IRepositoryManager repository) :
    IRequestHandler<GetArticleByIdQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(request.ArticleId))
            return OperationResult.Failure(OperationStatus.Invalid, Errors.InvalidId);

        // Retrieve the article
        var entity = await repository.Articles.GetByIdAsync(request.ArticleId);
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.ArticleNotFound);

        var tags = await repository.Tags.GetByIdsAsync(entity.TagIds);
        var model = entity.MapToModelWithTags(tags);

        return OperationResult.Success(model);
    }
}

// Model
public record GetArticleByIdQuery(string ArticleId) : IRequest<OperationResult>;