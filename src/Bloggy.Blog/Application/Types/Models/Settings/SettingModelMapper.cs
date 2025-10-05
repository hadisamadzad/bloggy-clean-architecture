using Bloggy.Blog.Application.Types.Entities;

namespace Bloggy.Blog.Application.Types.Models.Settings;

public static class SettingModelMapper
{
    public static SettingModel MapToModel(this SettingEntity entity)
    {
        return new SettingModel
        {
            BlogTitle = entity.BlogTitle,
            BlogSubtitle = entity.BlogSubtitle,
            BlogPageTitle = entity.BlogPageTitle,
            BlogDescription = entity.BlogDescription,
            SeoMetaTitle = entity.SeoMetaTitle,
            SeoMetaDescription = entity.SeoMetaDescription,
            BlogUrl = entity.BlogUrl,
            BlogLogoUrl = entity.BlogLogoUrl,
            Socials = entity.Socials,
            UpdatedAt = entity.UpdatedAt,
        };
    }

    public static IEnumerable<SettingModel> MapToModels(this IEnumerable<SettingEntity> entities)
    {
        foreach (var entity in entities)
            yield return entity.MapToModel();
    }
}
