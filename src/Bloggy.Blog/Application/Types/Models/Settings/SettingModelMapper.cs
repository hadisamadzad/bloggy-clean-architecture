using Bloggy.Blog.Application.Types.Entities;

namespace Bloggy.Blog.Application.Types.Models.Settings;

public static class SettingModelMapper
{
    public static SettingModel MapToModel(this SettingEntity entity)
    {
        return new SettingModel
        {
            AuthorName = entity.AuthorName,
            AuthorTitle = entity.AuthorTitle,
            AboutAuthor = entity.AboutAuthor,
            BlogTitle = entity.BlogTitle,
            BlogSubtitle = entity.BlogSubtitle,
            BlogDescription = entity.BlogDescription,

            BlogUrl = entity.BlogUrl,
            PageTitleTemplate = entity.PageTitleTemplate,
            SeoMetaTitle = entity.SeoMetaTitle,
            SeoMetaDescription = entity.SeoMetaDescription,
            BlogLogoUrl = entity.BlogLogoUrl,
            CopyrightText = entity.CopyrightText,
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
