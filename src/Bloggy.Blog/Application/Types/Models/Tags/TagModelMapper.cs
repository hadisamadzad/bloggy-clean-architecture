using Bloggy.Blog.Application.Types.Entities;

namespace Bloggy.Blog.Application.Types.Models.Tags;

public static class TagModelMapper
{
    public static TagModel MapToModel(this TagEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Slug = entity.Slug,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    public static IEnumerable<TagModel> MapToModels(this IEnumerable<TagEntity> entities)
    {
        foreach (var entity in entities)
            yield return entity.MapToModel();
    }
}