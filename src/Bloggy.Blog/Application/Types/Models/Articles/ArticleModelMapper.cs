using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.Types.Models.Tags;

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
            Tags = [.. entity.TagIds.Select(x => new TagModel { TagId = x })],
            OriginalArticleInfo = entity.OriginalArticleInfo,
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

        var tagModelMap = tagEntities.ToDictionary(x => x.Id, x => x.MapToModel());

        var tagModels = entity.TagIds
            .Where(tagModelMap.ContainsKey)
            .Select(tagId => tagModelMap[tagId])
            .ToList();

        return model with { Tags = tagModels };
    }
}
