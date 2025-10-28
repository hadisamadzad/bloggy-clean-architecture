using Bloggy.Core.Interfaces;

namespace Bloggy.Blog.Application.Types.Entities;

public class ArticleEntity : IDeletableEntity
{
    public required string Id { get; set; }
    public required string AuthorId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;

    public OriginalArticleInfoValue? OriginalArticleInfo { get; set; }

    public int TimeToReadInMinute { get; set; }
    public int Likes { get; set; }
    public int Views { get; set; }
    public ICollection<string> TagIds { get; set; } = [];

    public ArticleStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}