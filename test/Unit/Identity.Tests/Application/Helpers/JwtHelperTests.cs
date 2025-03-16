using System;
using System.IdentityModel.Tokens.Jwt;
using Common.Helpers;
using Identity.Application.Helpers;
using Identity.Application.Types.Configs;
using NSubstitute;
using Xunit;

namespace Identity.Tests.Application.Helpers;

public class JwtHelperTests
{
    private readonly AuthTokenConfig _config;

    public JwtHelperTests()
    {
        // Arrange
        _config = Substitute.For<AuthTokenConfig>();
        _config.Issuer = "bloggy.identity";
        _config.Audience = "bloggy.gateway";
        _config.AccessTokenSecretKey = RandomGenerator
            .GenerateString(64, AllowedCharacters.Alphanumeric);
        _config.RefreshTokenSecretKey = RandomGenerator
            .GenerateString(64, AllowedCharacters.Alphanumeric);
        _config.AccessTokenLifetime = TimeSpan.FromMinutes(30);
        _config.RefreshTokenLifetime = TimeSpan.FromDays(14);

        JwtHelper.Initialize(_config);
    }

    [Fact]
    public void TestCreateJwtAccessToken_ShouldReturnToken()
    {
        // Arrange
        const string userId = "userId123";
        const string email = "fake-email";

        // Act
        var token = JwtHelper.CreateJwtAccessToken(userId, email);
        var isValid = JwtHelper.IsValidJwtAccessToken(token);

        // Assert
        Assert.NotNull(token);
        Assert.True(isValid);
    }

    [Fact]
    public void TestCreateJwtRefreshToken_ShouldReturnToken()
    {
        // Arrange
        const string userId = "userId123";
        const string email = "user@example.com";

        // Act
        var token = JwtHelper.CreateJwtRefreshToken(userId, email);
        var isValid = JwtHelper.IsValidJwtRefreshToken(token);

        // Assert
        Assert.NotNull(token);
        Assert.True(isValid);
    }

    [Fact]
    public void TestIsValidJwtAccessToken_WhenInvalidTokenIsProvided_ReturnsFalse()
    {
        // Act
        var isValid = JwtHelper.IsValidJwtAccessToken("a-fake-jwt-token");

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void TestIsValidJwtRefreshToken_WhenInvalidTokenIsProvided_ReturnsFalse()
    {
        // Act
        var isValid = JwtHelper.IsValidJwtRefreshToken("a-fake-jwt-token");

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void TestIsValidJwtAccessToken_WhenValidRefreshTokenIsProvided_ReturnsFalse()
    {
        // Arrange
        const string userId = "userId123";
        const string email = "fake-email";

        // Act
        var token = JwtHelper.CreateJwtRefreshToken(userId, email);
        var isValid = JwtHelper.IsValidJwtAccessToken(token);

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void TestIsValidJwtRefreshToken_WhenValidAccessTokenIsProvided_ReturnsFalse()
    {
        // Arrange
        const string userId = "userId123";
        const string email = "fake-email";

        // Act
        var token = JwtHelper.CreateJwtAccessToken(userId, email);
        var isValid = JwtHelper.IsValidJwtRefreshToken(token);

        // Assert
        Assert.False(isValid);
    }
}
