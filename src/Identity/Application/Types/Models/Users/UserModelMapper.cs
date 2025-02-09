using Identity.Application.Helpers;
using Identity.Application.Types.Entities;

namespace Identity.Application.Types.Models.Users;

public static class UserModelMapper
{
    public static UserModel MapToUserModel(this UserEntity entity)
    {
        if (entity is null)
            return null;

        return new UserModel
        {
            UserId = entity.Id,
            Email = entity.Email,
            IsEmailConfirmed = entity.IsEmailConfirmed,
            Mobile = entity.Mobile,
            Role = entity.Role,
            State = entity.State,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            FullName = entity.GetFullName(),
            NotificationCount = 0,
            IsLockedOut = entity.IsLockedOut(),
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
