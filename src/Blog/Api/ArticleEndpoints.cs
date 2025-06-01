using Blog.Application.Types.Entities;
using Blog.Application.Types.Models.Articles;
using Blog.Application.UseCases.Articles;
using Common.Interfaces;
using Common.Utilities.OperationResult;
using Common.Utilities.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api;

public class ArticleEndpoints : IEndpoint
{
    const string Route = "api/articles/";
    const string Tag = "Articles";

    // Endpoints
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup(Route).WithTags(Tag);

        // Endpoint for creating an article
        group.MapPost("/", async (
            IMediator mediator,
            [FromHeader] string requestedBy,
            [FromBody] CreateArticleRequest request) =>
            {
                return await mediator.Send(new CreateArticleCommand
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
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (ArticleModel)operation.Value;
                return new
                {
                    ArticleId = value.Id,
                };
            });

        // Endpoint for getting an article
        group.MapGet("{articleId}/", async (
            IMediator mediator,
            [FromRoute] string articleId) =>
            {
                return await mediator.Send(new GetArticleByIdQuery(articleId));
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (ArticleModel)operation.Value;
                return new
                {
                    ArticleId = value.Id,
                    AuthorId = value.AuthorId,
                    Title = value.Title,
                    Subtitle = value.Subtitle,
                    Summary = value.Summary,
                    Content = value.Content,
                    Slug = value.Slug,
                    ThumbnailUrl = value.ThumbnailUrl,
                    CoverImageUrl = value.CoverImageUrl,
                    TimeToReadInMinute = value.TimeToReadInMinute,
                    Likes = value.Likes,
                    TagIds = value.TagIds,
                    Status = value.Status,
                    CreatedAt = value.CreatedAt,
                    UpdatedAt = value.UpdatedAt,
                    PublishedAt = value.PublishedAt,
                    ArchivedAt = value.ArchivedAt
                };
            });

        // Endpoint for getting a list of articles
        group.MapGet("/", async (
            IMediator mediator,
            [FromQuery] string? keyword,
            [FromQuery] string[]? tagIds,
            [FromQuery] ArticleStatus[]? statuses,
            [FromQuery] ArticleSortBy? sortBy,
            [FromQuery] int page,
            [FromQuery] int pageSize) =>
            {
                return await mediator.Send(new GetArticlesByFilterQuery(new()
                {
                    Keyword = keyword,
                    TagIds = [.. tagIds ?? []],
                    Statuses = [.. statuses ?? []],
                    SortBy = sortBy ?? ArticleSortBy.CreatedAtNewest,

                    Page = page,
                    PageSize = pageSize
                }));
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (PaginatedList<ArticleModel>)operation.Value;
                return new
                {
                    Page = value.Page,
                    PageSize = value.PageSize,
                    TotalCount = value.TotalCount,
                    Results = value.Results.Select(x => new
                    {
                        ArticleId = x.Id,
                        AuthorId = x.AuthorId,
                        Title = x.Title,
                        Subtitle = x.Subtitle,
                        Summary = x.Summary,
                        Content = x.Content,
                        Slug = x.Slug,
                        ThumbnailUrl = x.ThumbnailUrl,
                        CoverImageUrl = x.CoverImageUrl,
                        TimeToReadInMinute = x.TimeToReadInMinute,
                        Likes = x.Likes,
                        TagIds = x.TagIds,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt,
                        PublishedAt = x.PublishedAt,
                        ArchivedAt = x.ArchivedAt
                    })
                };
            });

        // Endpoint for updating an article
        group.MapPut("{articleId}/", async (
            IMediator mediator,
            [FromRoute] string articleId,
            [FromBody] UpdateArticleRequest request) =>
            {
                return await mediator.Send(new UpdateArticleCommand
                {
                    ArticleId = articleId,
                    Title = request.Title,
                    Subtitle = request.Subtitle,
                    Summary = request.Summary,
                    Content = request.Content,
                    Slug = request.Slug,
                    ThumbnailUrl = request.ThumbnailUrl,
                    CoverImageUrl = request.CoverImageUrl,
                    TimeToRead = request.TimeToRead,
                    TagIds = request.TagIds,
                });
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (ArticleModel)operation.Value;
                return new
                {
                    ArticleId = value.Id
                };
            });

        // Endpoint for updating status of an article
        group.MapPatch("{articleId}/status/", async (
            IMediator mediator,
            [FromRoute] string articleId,
            [FromBody] UpdateArticleStatusRequest request) =>
            {
                return await mediator.Send(new UpdateArticleStatusCommand
                (
                    ArticleId: articleId,
                    Status: request.Status
                ));
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (ArticleModel)operation.Value;
                return new
                {
                    ArticleId = value.Id,
                    Status = value.Status
                };
            });

        // Endpoint for deleting an article
        group.MapDelete("{articleId}/", async (
            IMediator mediator,
            [FromRoute] string articleId) =>
            {
                return await mediator.Send(new DeleteArticleCommand(ArticleId: articleId));
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (ArticleModel)operation.Value;
                return new
                {
                    ArticleId = value.Id
                };
            });
    }
}

// Models
public record CreateArticleRequest(
    string Title,
    string? Subtitle,
    string? Summary,
    string? Content,
    string? Slug,
    string? ThumbnailUrl,
    string? CoverImageUrl,
    ICollection<string> TagIds);

public record UpdateArticleRequest(
    string Title,
    string Subtitle,
    string Summary,
    string Content,
    string Slug,
    string ThumbnailUrl,
    string CoverImageUrl,
    int TimeToRead,
    ICollection<string> TagIds);

public record UpdateArticleStatusRequest(ArticleStatus Status);