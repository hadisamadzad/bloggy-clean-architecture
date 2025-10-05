using Bloggy.Core.Interfaces;

namespace Bloggy.Blog.Application.Types.Entities;

public class SettingEntity : IEntity
{
    public required string Id { get; set; } = "blog_settings";
    public required string BlogTitle { get; set; }
    public required string BlogSubtitle { get; set; }
    public required string BlogPageTitle { get; set; }
    public string BlogDescription { get; set; } = string.Empty;
    public string SeoMetaTitle { get; set; } = string.Empty;
    public string SeoMetaDescription { get; set; } = string.Empty;
    public string BlogUrl { get; set; } = string.Empty;
    public string BlogLogoUrl { get; set; } = string.Empty;
    public ICollection<SocialNetwork> Socials { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
}

public record SocialNetwork
{
    public int Order { get; set; }
    public SocialNetworkName Name { get; set; }
    public string? Url { get; set; }
}