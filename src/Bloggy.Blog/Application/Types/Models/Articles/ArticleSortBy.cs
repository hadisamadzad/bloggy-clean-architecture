namespace Bloggy.Blog.Application.Types.Models.Articles;

public enum ArticleSortBy
{
    CreatedAtNewest = 1,
    CreatedAtOldest,
    UpdatedAtNewest,
    UpdatedAtOldest,
    PublishedAtNewest,
    PublishedAtOldest,
    ArchivedAtNewest,
    ArchivedAtOldest,

    LikesMost,
    LikesFewest,
}