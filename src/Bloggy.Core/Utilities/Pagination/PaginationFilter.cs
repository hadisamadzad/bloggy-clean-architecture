namespace Bloggy.Core.Utilities.Pagination;

public record PaginationFilter
{
    private int page;
    private int pageSize;

    private void SetPage(int value) =>
        page = value >= 1 ? value : throw new ArgumentException("Invalid page number!");

    private void SetPageSize(int value) =>
        pageSize = value >= 1 ? value : throw new ArgumentException("Invalid page size!");

    public int Page { get => page; set => SetPage(value); }
    public int PageSize { get => pageSize; set => SetPageSize(value); }
    public bool HasPagination { get; set; } = true;
}