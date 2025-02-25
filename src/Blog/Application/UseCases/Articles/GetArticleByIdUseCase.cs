using Blog.Application.Constants;
using Blog.Application.Interfaces;
using Blog.Application.Types.Models.Articles;
using Common.Utilities.OperationResult;
using MediatR;

namespace Blog.Application.UseCases.Articles;

// Handler
internal class GetArticleByIdHandler(IRepositoryManager repository) :
    IRequestHandler<GetArticleByIdQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetArticleByIdQuery request, CancellationToken cancel)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(request.ArticleId))
            return OperationResult.Failure(OperationStatus.Invalid, Errors.InvalidId);

        // Retrieve the article
        var entity = await repository.Articles.GetByIdAsync(request.ArticleId);
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.ArticleNotFound);

        var model = entity.MapToModel();

        return OperationResult.Success(model);
    }
}

// Model
public record GetArticleByIdQuery(string ArticleId) : IRequest<OperationResult>;