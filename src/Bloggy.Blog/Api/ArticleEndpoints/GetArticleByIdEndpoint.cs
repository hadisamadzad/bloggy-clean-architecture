using Bloggy.Blog.Api.TagEndpoints;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Articles;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api.ArticleEndpoints;

public class GetArticleByIdEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for getting an article by id
        app.MapGroup(Routes.ArticleBaseRoute)
            .WithSummary("Get a Protected Article by Id")
            .MapGet("{articleId}/", async (IOperationService operations,
                [FromRoute] string articleId) =>
            {
                // Operation
                var operationResult = await operations.GetArticleById
                    .ExecuteAsync(new GetArticleByIdCommand(articleId));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new GetArticleResponse(
                            ArticleId: operationResult.Value!.Id,
                            AuthorId: operationResult.Value!.AuthorId,
                            Title: operationResult.Value!.Title,
                            Subtitle: operationResult.Value!.Subtitle,
                            Summary: operationResult.Value!.Summary,
                            Content: operationResult.Value!.Content,
                            Slug: operationResult.Value!.Slug,
                            ThumbnailUrl: operationResult.Value!.ThumbnailUrl,
                            CoverImageUrl: operationResult.Value!.CoverImageUrl,
                            TimeToReadInMinute: operationResult.Value!.TimeToReadInMinute,
                            Likes: operationResult.Value!.Likes,
                            Tags: [.. operationResult.Value!.Tags.Select(tag => new TagResponse(tag.TagId, tag.Name, tag.Slug))],
                            Status: operationResult.Value!.Status,
                            CreatedAt: operationResult.Value!.CreatedAt,
                            UpdatedAt: operationResult.Value!.UpdatedAt,
                            PublishedAt: operationResult.Value!.PublishedAt,
                            ArchivedAt: operationResult.Value!.ArchivedAt
                        )),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.ArticleEndpointGroupTag)
            .WithDescription("Gets a protected article by id.")
            .Produces<GetArticleResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}