using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Views;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api.VisitorEndpoints;

public class CountArticleViewEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGroup(Routes.ViewBaseRoute)
            .WithSummary("Track Article View by Visitor")
            .MapPost("article/{articleId}/", async (IOperationService operations,
                [FromRoute] string articleId,
                [FromBody] CountArticleViewRequest request) =>
            {
                var operationResult = await operations.CountArticleView.ExecuteAsync(
                    new CountArticleViewCommand(articleId, request.VisitorId));

                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags(Routes.ViewEndpointGroupTag)
            .WithDescription("Counts a unique view for an article by visitor id.");
    }
}

public record CountArticleViewRequest(string VisitorId);