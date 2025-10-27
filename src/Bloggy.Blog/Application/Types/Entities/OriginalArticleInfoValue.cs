namespace Bloggy.Blog.Application.Types.Entities;

public record OriginalArticleInfoValue
{
    public required string Platform { get; init; }
    public required string Url { get; init; }
    public required DateOnly PublishedOn { get; init; }
}