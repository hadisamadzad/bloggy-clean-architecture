using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Utilities.Pagination;

namespace Bloggy.Blog.Application.Types.Models.Articles;

public record ArticleFilter : PaginationFilter
{
    public string? Keyword { get; set; }
    public List<string> TagIds { get; set; } = [];
    public List<ArticleStatus> Statuses { get; set; } = [];
    public ArticleSortBy SortBy { get; set; } = ArticleSortBy.CreatedAtNewest;
}
