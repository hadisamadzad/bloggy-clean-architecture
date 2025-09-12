namespace Bloggy.Gateway.Core.Configs;

public record JwtTokenConfig
{
    public const string Key = "JwtSecurityToken";

    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
}