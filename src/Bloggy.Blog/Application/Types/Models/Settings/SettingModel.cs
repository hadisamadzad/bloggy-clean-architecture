using Bloggy.Blog.Application.Types.Entities;

namespace Bloggy.Blog.Application.Types.Models.Settings;

public class SettingModel
{
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