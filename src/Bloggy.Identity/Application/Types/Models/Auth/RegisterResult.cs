namespace Bloggy.Identity.Application.Types.Models.Auth;

public record RegisterResult
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string ActivationToken { get; set; }
}