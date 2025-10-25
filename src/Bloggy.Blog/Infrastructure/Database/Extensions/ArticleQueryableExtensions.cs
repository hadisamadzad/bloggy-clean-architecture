using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.Types.Models.Articles;
using MongoDB.Driver.Linq;

namespace Bloggy.Blog.Infrastructure.Database.Extensions;

public static class ArticleQueryableExtensions
{
    public static IQueryable<ArticleEntity> ApplyFilter(this IQueryable<ArticleEntity> query,
        ArticleFilter filter)
    {
        // Filter by keyword
        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            var keyword = filter.Keyword.Trim().ToLower();
            query = query.Where(x =>
                x.Title.ToLower().Contains(keyword) ||
                x.Subtitle.ToLower().Contains(keyword) ||
                x.Summary.ToLower().Contains(keyword) ||
                x.Content.ToLower().Contains(keyword));
        }

        // Filter by tag ids
        if (filter.TagIds.Count > 0)
            query = query.Where(x => x.TagIds.Any(tagId => filter.TagIds.Contains(tagId)));

        // Filter by statuses
        if (filter.Statuses.Count > 0)
            query = query.Where(x => filter.Statuses.Contains(x.Status));

        return query;
    }

    public static IQueryable<ArticleEntity> ApplySort(this IQueryable<ArticleEntity> query,
        ArticleSortBy? sortBy) => sortBy switch
        {
            ArticleSortBy.CreatedAtNewest => query.OrderByDescending(x => x.CreatedAt),
            ArticleSortBy.CreatedAtOldest => query.OrderBy(x => x.CreatedAt),
            ArticleSortBy.UpdatedAtNewest => query.OrderByDescending(x => x.UpdatedAt),
            ArticleSortBy.UpdatedAtOldest => query.OrderBy(x => x.UpdatedAt),
            ArticleSortBy.PublishedAtNewest => query.OrderByDescending(x => x.PublishedAt),
            ArticleSortBy.PublishedAtOldest => query.OrderBy(x => x.PublishedAt),
            ArticleSortBy.ArchivedAtNewest => query.OrderByDescending(x => x.ArchivedAt),
            ArticleSortBy.ArchivedAtOldest => query.OrderBy(x => x.ArchivedAt),
            ArticleSortBy.LikesMost => query.OrderByDescending(x => x.Likes),
            ArticleSortBy.LikesFewest => query.OrderBy(x => x.Likes),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };
}
