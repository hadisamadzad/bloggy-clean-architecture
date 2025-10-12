using Bloggy.Identity.Application.Types.Entities;

namespace Bloggy.Identity.Application.Types.Models.Users;

public record UserModel
{
    public string UserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public string? Mobile { get; set; }

    public DateTime? LastLoginDate { get; set; }
    public DateTime? LastPasswordChangeDate { get; set; }
    public bool IsLockedOut { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Role Role { get; set; }
    public UserState Status { get; set; }
    public long NotificationCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}