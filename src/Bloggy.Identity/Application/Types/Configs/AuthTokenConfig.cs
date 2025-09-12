namespace Bloggy.Identity.Application.Types.Configs;

public record AuthTokenConfig
{
    public const string Key = "AuthToken";

    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string AccessTokenSecretKey { get; set; }
    public TimeSpan AccessTokenLifetime { get; set; }
    public string RefreshTokenSecretKey { get; set; }
    public TimeSpan RefreshTokenLifetime { get; set; }
}