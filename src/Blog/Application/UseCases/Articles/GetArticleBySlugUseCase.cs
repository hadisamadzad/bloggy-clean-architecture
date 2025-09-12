using Blog.Application.Constants;
using Blog.Application.Interfaces;
using Blog.Application.Types.Models.Articles;
using Common.Utilities.OperationResult;
using MediatR;

namespace Blog.Application.UseCases.Articles;

// Handler
public class GetArticleBySlugHandler(IRepositoryManager repository) :
    IRequestHandler<GetArticleBySlugQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetArticleBySlugQuery request, CancellationToken cancellationToken)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(request.Slug))
            return OperationResult.Failure(OperationStatus.Invalid, Errors.InvalidId);

        request = request with { Slug = request.Slug.ToLower() };

        // Retrieve the article
        var entity = await repository.Articles.GetBySlugAsync(request.Slug);
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.ArticleNotFound);

        var tags = await repository.Tags.GetByIdsAsync(entity.TagIds);
        var model = entity.MapToModelWithTags(tags);

        return OperationResult.Success(model);
    }
}

// Model
public record GetArticleBySlugQuery(string Slug) : IRequest<OperationResult>;