using Bloggy.Blog.Application.Types.Entities;

namespace Bloggy.Blog.Application.Types.Models.Settings;

public class SettingModel
{
    // Author
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorTitle { get; set; } = string.Empty;
    public string AboutAuthor { get; set; } = string.Empty;


    // Blog
    public required string BlogTitle { get; set; }
    public required string BlogSubtitle { get; set; }
    public string BlogDescription { get; set; } = string.Empty;

    // SEO Settings
    public string BlogUrl { get; set; } = string.Empty;
    public required string PageTitleTemplate { get; set; }
    public string SeoMetaTitle { get; set; } = string.Empty;
    public string SeoMetaDescription { get; set; } = string.Empty;

    public string BlogLogoUrl { get; set; } = string.Empty;
    public string CopyrightText { get; set; } = string.Empty;

    public ICollection<SocialNetwork> Socials { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
}