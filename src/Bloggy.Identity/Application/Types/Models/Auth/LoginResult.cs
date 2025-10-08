namespace Bloggy.Identity.Application.Types.Models.Auth;

public record LoginResult
(
    string Email,
    string FullName,
    string AccessToken,
    string RefreshToken,
    TimeSpan RefreshTokenMaxAge
);