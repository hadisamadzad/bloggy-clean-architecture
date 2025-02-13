﻿using Blog.Application.Types.Entities;

namespace Blog.Application.Types.Models.Articles;

public record ArticleModel
{
    public string Id { get; init; } = string.Empty;
    public string AuthorId { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string ThumbnailUrl { get; init; } = string.Empty;
    public string CoverImageUrl { get; init; } = string.Empty;

    public int TimeToReadInMinute { get; init; }
    public int Likes { get; init; }
    public ICollection<string> TagIds { get; init; } = [];

    public ArticleStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? PublishedAt { get; init; }
    public DateTime? ArchivedAt { get; init; }
}