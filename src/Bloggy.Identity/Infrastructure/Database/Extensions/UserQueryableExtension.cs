using Bloggy.Identity.Application.Types.Entities;
using Bloggy.Identity.Application.Types.Models.Users;

namespace Bloggy.Identity.Infrastructure.Database.Extensions;

public static class UserQueryableExtension
{
    public static IQueryable<UserEntity> ApplyFilter(this IQueryable<UserEntity> query, UserFilter filter)
    {
        // Filter by keyword
        if (!string.IsNullOrEmpty(filter.Keyword))
            query = query.Where(x =>
                x.FirstName.ToLower().Contains(filter.Keyword.ToLower().Trim()) ||
                x.LastName.ToLower().Contains(filter.Keyword.ToLower().Trim()) ||
                x.Email.ToLower().Contains(filter.Keyword.ToLower().Trim()) ||
                x.Mobile.ToLower().Contains(filter.Keyword.ToLower().Trim()));

        // Filter by email
        if (!string.IsNullOrEmpty(filter.Email))
            query = query.Where(x => x.Email.ToLower().Contains(filter.Email.ToLower().Trim()));

        // Filter by statuses
        if (filter.States?.Any() == true)
            query = query.Where(x => filter.States.Contains(x.State));

        return query;
    }

    public static IQueryable<UserEntity> ApplySort(this IQueryable<UserEntity> query, UserSortBy? sortBy)
    {
        return sortBy switch
        {
            UserSortBy.CreationDate => query.OrderBy(x => x.CreatedAt),
            UserSortBy.CreationDateDescending => query.OrderByDescending(x => x.CreatedAt),
            UserSortBy.LastLoginDate => query.OrderBy(x => x.LastLoginDate),
            UserSortBy.LastLoginDateDescending => query.OrderByDescending(x => x.LastLoginDate),
            _ => query.OrderByDescending(x => x.Id)
        };
    }
}
