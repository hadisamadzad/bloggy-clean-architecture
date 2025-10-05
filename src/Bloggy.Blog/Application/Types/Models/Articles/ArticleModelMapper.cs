using Bloggy.Blog.Application.Types.Entities;

namespace Bloggy.Blog.Application.Types.Models.Articles;

public static class ArticleModelMapper
{
    public static ArticleModel MapToModel(this ArticleEntity entity)
    {
        return new ArticleModel
        {
            Id = entity.Id,
            AuthorId = entity.AuthorId,

            Title = entity.Title,
            Subtitle = entity.Subtitle,
            Summary = entity.Summary,
            Content = entity.Content,
            Slug = entity.Slug,
            ThumbnailUrl = entity.ThumbnailUrl,
            CoverImageUrl = entity.CoverImageUrl,

            TimeToReadInMinute = entity.TimeToReadInMinute,
            Likes = entity.Likes,
            TagIds = entity.TagIds,
            TagSlugs = [],

            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            PublishedAt = entity.PublishedAt,
            ArchivedAt = entity.ArchivedAt,
        };
    }

    public static IEnumerable<ArticleModel> MapToModels(this IEnumerable<ArticleEntity> entities)
    {
        foreach (var entity in entities)
            yield return entity.MapToModel();
    }

    public static ArticleModel MapToModelWithTags(this ArticleEntity entity, IEnumerable<TagEntity> tagEntities)
    {
        var model = entity.MapToModel();

        var tagSlugMap = tagEntities.ToDictionary(x => x.Id, x => x.Slug);

        var slugs = entity.TagIds
            .Where(tagSlugMap.ContainsKey)
            .Select(tagId => tagSlugMap[tagId])
            .ToList();

        return model with { TagSlugs = slugs };
    }
}
