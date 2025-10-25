using Bloggy.Core.Interfaces;

namespace Bloggy.Blog.Application.Types.Entities;

public class SettingEntity : IEntity
{
    public required string Id { get; set; } = "blog_settings";

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

public record SocialNetwork
{
    public int Order { get; set; }
    public SocialNetworkName Name { get; set; }
    public string? Url { get; set; }
}