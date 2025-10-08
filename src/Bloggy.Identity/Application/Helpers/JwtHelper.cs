using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bloggy.Identity.Application.Types.Configs;
using Microsoft.IdentityModel.Tokens;

namespace Bloggy.Identity.Application.Helpers;

public static class JwtHelper
{
    private static AuthTokenConfig Config;

    public static void Initialize(AuthTokenConfig config)
    {
        Config = config;
    }

    public static string CreateJwtAccessToken(string userId, string email) =>
        CreateJwt(Config.AccessTokenSecretKey, Config.AccessTokenLifetime, userId, email);

    public static string CreateJwtRefreshToken(string userId, string email) =>
        CreateJwt(Config.RefreshTokenSecretKey, Config.RefreshTokenLifetime, userId, email);

    public static TimeSpan RefreshTokenMaxAge => Config.RefreshTokenLifetime;

    public static bool IsValidJwtAccessToken(string token) =>
        ValidateJwt(token, Config.AccessTokenSecretKey);

    public static bool IsValidJwtRefreshToken(string token) =>
        ValidateJwt(token, Config.RefreshTokenSecretKey);

    public static string GetEmail(string token)
    {
        var payload = GetPayload(token);
        return payload.Claims.SingleOrDefault(x => x.Type == "unique_name")?.Value;
    }

    private static string CreateJwt(string key, TimeSpan lifetime, string userId, string email)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.UniqueName, email.ToLower())
        };

        var token = new JwtSecurityToken(
            signingCredentials: credentials,
            issuer: Config.Issuer,
            audience: Config.Audience,
            claims: [.. claims],
            expires: DateTime.UtcNow.Add(lifetime)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool ValidateJwt(string token, string securityKey)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        try
        {
            _ = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
                ValidateIssuerSigningKey = true,
                ValidIssuer = Config.Issuer,
                ValidAudience = Config.Audience
            }, out SecurityToken validatedToken);

            return validatedToken != null;
        }
        catch
        {
            return false;
        }
    }

    private static JwtPayload GetPayload(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        return new JwtSecurityTokenHandler().ReadJwtToken(token).Payload;
    }
}