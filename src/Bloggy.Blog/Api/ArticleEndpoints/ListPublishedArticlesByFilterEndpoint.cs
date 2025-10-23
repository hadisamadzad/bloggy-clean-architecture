using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Articles;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Core.Utilities.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api.ArticleEndpoints;

public class ListPublishedArticlesByFilterEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for getting a list of articles
        app.MapGroup(Routes.ArticleBaseRoute)
            .WithSummary("Get Public Published Articles by Filter")
            .MapGet("published/", async (IOperationService operations,
                [FromQuery] string? keyword,
                [FromQuery] string[]? tagIds,
                [FromQuery] ArticleSortBy? sortBy,
                [FromQuery] int page,
                [FromQuery] int pageSize) =>
            {
                // Operation
                var operationResult = await operations.GetArticlesByFilter
                    .ExecuteAsync(new GetArticlesByFilterCommand(new()
                    {
                        Keyword = keyword,
                        TagIds = [.. tagIds ?? []],
                        Statuses = [ArticleStatus.Published],
                        SortBy = sortBy ?? ArticleSortBy.CreatedAtNewest,

                        Page = page,
                        PageSize = pageSize
                    }));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(new PaginatedList<GetArticleResponse>
                    {
                        Page = operationResult.Value!.Page,
                        PageSize = operationResult.Value!.PageSize,
                        TotalCount = operationResult.Value!.TotalCount,
                        Results = [.. operationResult.Value!.Results.Select(x => new GetArticleResponse(
                            ArticleId: x.Id,
                            AuthorId: x.AuthorId,
                            Title: x.Title,
                            Subtitle: x.Subtitle,
                            Summary: x.Summary,
                            Content: string.Empty, // Omit content in list response
                            Slug: x.Slug,
                            ThumbnailUrl: x.ThumbnailUrl,
                            CoverImageUrl: x.CoverImageUrl,
                            TimeToReadInMinute: x.TimeToReadInMinute,
                            Likes: x.Likes,
                            TagIds: x.TagIds,
                            TagSlugs: x.TagSlugs,
                            Status: x.Status,
                            CreatedAt: x.CreatedAt,
                            UpdatedAt: x.UpdatedAt,
                            PublishedAt: x.PublishedAt,
                            ArchivedAt: x.ArchivedAt
                        ))]
                    }),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.ArticleEndpointGroupTag)
            .WithDescription("Gets a public list of published articles based on filter criteria.")
            .Produces<PaginatedList<GetArticleResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}