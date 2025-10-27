using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Articles;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api.ArticleEndpoints;

public class UpdateArticleEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for updating an article
        app.MapGroup(Routes.ArticleBaseRoute)
            .WithSummary("Update Article")
            .MapPut("{articleId}/", async (IOperationService operations,
                [FromRoute] string articleId,
                [FromBody] UpdateArticleRequest request) =>
            {
                // Operation
                var operationResult = await operations.UpdateArticle.ExecuteAsync(new UpdateArticleCommand
                {
                    ArticleId = articleId,
                    Title = request.Title,
                    Subtitle = request.Subtitle,
                    Summary = request.Summary,
                    Content = request.Content,
                    Slug = request.Slug,
                    ThumbnailUrl = request.ThumbnailUrl,
                    CoverImageUrl = request.CoverImageUrl,
                    OriginalArticleInfo = request.OriginalArticleInfo,
                    TimeToRead = request.TimeToRead,
                    TagIds = request.TagIds,
                });

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.Failed => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.ArticleEndpointGroupTag)
            .WithDescription("Updates an existing article.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record UpdateArticleRequest(
    string Title,
    string Subtitle,
    string Summary,
    string Content,
    string Slug,
    string ThumbnailUrl,
    string CoverImageUrl,
    OriginalArticleInfoValue? OriginalArticleInfo,
    int TimeToRead,
    ICollection<string> TagIds);