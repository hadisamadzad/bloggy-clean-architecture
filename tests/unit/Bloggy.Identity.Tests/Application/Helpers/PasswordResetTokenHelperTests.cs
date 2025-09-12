using System;
using Bloggy.Core.Helpers;
using Bloggy.Identity.Application.Helpers;
using Xunit;

namespace Bloggy.Identity.Tests.Application.Helpers;

public class PasswordResetTokenHelperTests
{
    public PasswordResetTokenHelperTests()
    {
        PasswordResetTokenHelper.SetEncryptionKey(
            RandomGenerator.GenerateString(16, AllowedCharacters.Alphanumeric));
    }

    [Fact]
    public void TestGeneratePasswordResetToken_ShouldGeneratePasswordResetToken()
    {
        // Arrange
        const string fakeEmail = "fake@email.com";
        var expiration = DateTime.UtcNow.Date.AddDays(1);

        // Act
        var token = PasswordResetTokenHelper.GeneratePasswordResetToken(fakeEmail, expiration);

        // Assert
        Assert.NotEmpty(token);
    }

    [Fact]
    public void TestReadPasswordResetToken_ShouldExtractEmail_WhenValidTokenIsProvided()
    {
        // Arrange
        const string fakeEmail = "fake@email.com";
        var expiration = DateTime.UtcNow.Date.AddDays(1);
        var token = PasswordResetTokenHelper.GeneratePasswordResetToken(fakeEmail, expiration);

        // Act
        var (succeeded, email) = PasswordResetTokenHelper.ReadPasswordResetToken(token);

        // Assert
        Assert.True(succeeded);
        Assert.Equal(fakeEmail, email);
    }

    [Fact]
    public void TestReadPasswordResetToken_ShouldExtractEmail_WhenValidTokenIsProvided2()
    {
        // Arrange
        const string fakeEmail = "fake@email.com";
        var expiration = DateTime.UtcNow.Date;
        var token = PasswordResetTokenHelper.GeneratePasswordResetToken(fakeEmail, expiration);

        // Act
        var (succeeded, email) = PasswordResetTokenHelper.ReadPasswordResetToken(token);

        // Assert
        Assert.False(succeeded);
        Assert.Empty(email);
    }

    [Fact]
    public void TestReadPasswordResetToken_ShouldReturnFailure_WhenInvalidTokenType()
    {
        // Arrange
        var token = "invalidBase64UrlToken";

        // Assert
        Assert.Throws<FormatException>(
            () => PasswordResetTokenHelper.ReadPasswordResetToken(token));
    }
}
