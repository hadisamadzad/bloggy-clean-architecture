using Bloggy.Core.Utilities.Pagination;
using Bloggy.Identity.Application.Types.Entities;

namespace Bloggy.Identity.Application.Types.Models.Users;

public record UserFilter : PaginationFilter
{
    public string Keyword { get; init; }
    public string Email { get; init; }
    public List<UserState> States { get; init; }

    public UserSortBy? SortBy { get; init; }
}
