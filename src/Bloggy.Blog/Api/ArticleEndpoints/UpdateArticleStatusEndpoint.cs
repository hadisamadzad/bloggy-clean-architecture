using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Articles;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api.ArticleEndpoints;

public class UpdateArticleStatusEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for updating status of an article
        app.MapGroup(Routes.ArticleBaseRoute)
            .WithSummary("Update Article Status")
            .MapPatch("{articleId}/status/", async (IOperationService operations,
                [FromRoute] string articleId,
                [FromBody] UpdateArticleStatusRequest request) =>
            {
                // Operation
                var operationResult = await operations.UpdateArticleStatus.ExecuteAsync(new UpdateArticleStatusCommand
                (
                    ArticleId: articleId,
                    Status: request.Status
                ));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.ArticleEndpointGroupTag)
            .WithDescription("Updates the status of an existing article.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record UpdateArticleStatusRequest(ArticleStatus Status);