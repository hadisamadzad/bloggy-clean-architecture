using Bloggy.Core.Interfaces;

namespace Bloggy.Identity.Application.Types.Entities;

public class UserEntity : IEntity
{
    #region Identity

    public string Id { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }

    public string Mobile { get; set; }

    #endregion

    #region Login

    public string PasswordHash { get; set; }
    public DateTime? LastPasswordChangeTime { get; set; }

    public int FailedLoginCount { get; set; }
    public DateTime? LockoutEndTime { get; set; }

    public DateTime? LastLoginDate { get; set; }

    #endregion

    #region Profile

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Role Role { get; set; }
    public UserState State { get; set; }

    #endregion

    #region Management

    public string SecurityStamp { get; set; }
    public string ConcurrencyStamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    #endregion
}