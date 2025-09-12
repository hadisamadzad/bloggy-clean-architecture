using Bloggy.Identity.Application.Types.Entities;

namespace Bloggy.Identity.Application.Types.Models.Users;

public record UserModel
{
    public string UserId { get; set; }

    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string Mobile { get; set; }


    public DateTime? LastPasswordChangeDate { get; set; }
    public bool IsLockedOut { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public Role Role { get; set; }
    public UserState State { get; set; }
    public long NotificationCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}