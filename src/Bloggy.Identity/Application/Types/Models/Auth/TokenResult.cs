namespace Bloggy.Identity.Application.Types.Models.Auth;

public record TokenResult
{
    public string AccessToken { get; set; }
}