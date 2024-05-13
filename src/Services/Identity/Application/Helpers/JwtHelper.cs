﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Communal.Api.Infrastructure;
using Identity.Application.Types.Configs;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Application.Helpers;

public static class JwtHelper
{
    public static SecurityTokenConfig Config;

    public static string CreateJwtAccessToken(int userId, string email) =>
        CreateJwt(Config.AccessTokenSecretKey, Config.AccessTokenLifetime, userId, email);

    public static string CreateJwtRefreshToken(int userId, string email) =>
        CreateJwt(Config.RefreshTokenSecretKey, Config.RefreshTokenLifetime, userId, email);

    public static bool IsValidJwtAccessToken(string token) =>
        ValidateJwt(token, Config.AccessTokenSecretKey);

    public static bool IsValidJwtRefreshToken(string token) =>
        ValidateJwt(token, Config.RefreshTokenSecretKey);

    public static string GetEmail(string token)
    {
        var payload = GetPayload(token);
        return payload.Claims.SingleOrDefault(x => x.Type == "unique_name")?.Value;
    }

    // Private methods
    private static string CreateJwt(string key, TimeSpan lifetime, int userId, string email)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.Encode()),
            new(JwtRegisteredClaimNames.UniqueName, email.ToLower())
        };

        var token = new JwtSecurityToken(
            signingCredentials: credentials,
            issuer: Config.Issuer,
            audience: Config.Audience,
            claims: claims.ToArray(),
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