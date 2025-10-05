namespace Bloggy.Identity.Application.Types.Models.Auth;

public record LoginResult
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}