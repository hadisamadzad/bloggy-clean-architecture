using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Articles;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api.ArticleEndpoints;

public class CreateArticleEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for creating an article
        app.MapGroup(Routes.ArticleBaseRoute)
            .WithSummary("Create Article")
            .MapPost("/", async (IOperationService operations,
                [FromHeader] string requestedBy,
                [FromBody] CreateArticleRequest request) =>
            {
                // Operation
                var operationResult = await operations.CreateArticle.ExecuteAsync(
                    new CreateArticleCommand
                    {
                        AuthorId = requestedBy,
                        Title = request.Title,
                        Subtitle = request.Subtitle ?? string.Empty,
                        Summary = request.Summary ?? string.Empty,
                        Content = request.Content ?? string.Empty,
                        Slug = request.Slug ?? string.Empty,
                        ThumbnailUrl = request.ThumbnailUrl ?? string.Empty,
                        CoverImageUrl = request.CoverImageUrl ?? string.Empty,
                        TagIds = request.TagIds
                    });

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new CreateArticleResponse(
                            Slug: operationResult.Value!
                        )),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.Failed => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.ArticleEndpointGroupTag)
            .WithDescription("Creates a new article.")
            .Produces<CreateArticleResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record CreateArticleRequest(
    string Title,
    string? Subtitle,
    string? Summary,
    string? Content,
    string? Slug,
    string? ThumbnailUrl,
    string? CoverImageUrl,
    ICollection<string> TagIds);

public record CreateArticleResponse(string Slug);