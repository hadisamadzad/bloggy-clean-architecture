using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Types.Entities;

namespace Bloggy.Identity.Application.Types.Models.Users;

public static class UserModelMapper
{
    public static UserModel MapToUserModel(this UserEntity entity)
    {
        return new UserModel
        {
            UserId = entity.Id,
            Email = entity.Email,
            IsEmailConfirmed = entity.IsEmailConfirmed,
            Mobile = entity.Mobile,
            Role = entity.Role,
            Status = entity.Status,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            FullName = entity.GetFullName(),
            NotificationCount = 0,
            IsLockedOut = entity.IsLockedOut(),
            LastLoginDate = entity.LastLoginDate,
            LastPasswordChangeDate = entity.LastPasswordChangeTime,

            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }

    public static IEnumerable<UserModel> MapToUserModels(this IEnumerable<UserEntity> entities)
    {
        foreach (var entity in entities)
            yield return entity.MapToUserModel();
    }
}
