using System.Threading;
using System.Threading.Tasks;
using Blog.Application.Constants;
using Blog.Application.Interfaces;
using Blog.Application.Types.Entities;
using Blog.Application.UseCases.Settings;
using Common.Utilities;
using Common.Utilities.OperationResult;
using NSubstitute;
using Xunit;

namespace Blog.Tests.Application.UseCases.Settings;

public class UpdateBlogSettingsHandlerTests
{
    private readonly IRepositoryManager _repository;
    private readonly UpdateBlogSettingsHandler _handler;

    readonly UpdateBlogSettingsCommand ValidCommand = new()
    {
        BlogTitle = "Updated Blog Title",
        BlogDescription = "Updated Description",
        SeoMetaTitle = "Updated SEO Title",
        SeoMetaDescription = "Updated SEO Description",
        BlogUrl = "https://updated-blog.com",
        BlogLogoUrl = "https://updated-blog.com/logo.png",
        Socials =
            [
                new SocialNetwork
                {
                    Name = SocialNetworkName.Twitter,
                    Url = "https://twitter.com"
                }
            ]
    };

    public UpdateBlogSettingsHandlerTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _handler = new UpdateBlogSettingsHandler(_repository);
    }

    [Fact]
    public async Task TestHandle_WhenSettingsNotFound_ShouldReturnUnprocessable()
    {
        // Arrange
        var command = ValidCommand;

        _repository.Settings.GetBlogSettingAsync().Returns((SettingEntity)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Unprocessable, result.Status);
        Assert.Equal(Errors.SettingsNotFound, result.Value);
    }

    [Fact]
    public async Task TestHandle_WhenValidRequest_ShouldUpdateSettings()
    {
        // Arrange
        var command = new UpdateBlogSettingsCommand
        {
            BlogTitle = "Updated Blog Title",
            BlogDescription = "Updated Description",
            SeoMetaTitle = "Updated SEO Title",
            SeoMetaDescription = "Updated SEO Description",
            BlogUrl = "https://updated-blog.com",
            BlogLogoUrl = "https://updated-blog.com/logo.png",
            Socials =
            [
                new SocialNetwork
                {
                    Name = SocialNetworkName.Twitter,
                    Url = "https://twitter.com"
                }
            ]
        };

        var existingSettings = new SettingEntity
        {
            Id = "settings-1",
            BlogTitle = string.Empty
        };
        _repository.Settings.GetBlogSettingAsync().Returns(existingSettings);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(OperationStatus.Completed, result.Status);
        Assert.Equal(command.BlogTitle, existingSettings.BlogTitle);
        Assert.Equal(command.BlogDescription, existingSettings.BlogDescription);
        Assert.Equal(command.SeoMetaTitle, existingSettings.SeoMetaTitle);
        Assert.Equal(command.SeoMetaDescription, existingSettings.SeoMetaDescription);
        Assert.Equal(command.BlogUrl, existingSettings.BlogUrl);
        Assert.Equal(command.BlogLogoUrl, existingSettings.BlogLogoUrl);
        Assert.Equal(command.Socials.Count, existingSettings.Socials.Count);
        await _repository.Settings.Received(1).UpdateAsync(existingSettings);
    }

    [Fact]
    public async Task TestHandle_WhenInvalidBlogTitle_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with { BlogTitle = string.Empty };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var error = result.Value as ErrorModel;

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.Equal("Invalid blog title.", error.Message);
    }

    [Fact]
    public async Task TestHandle_WhenInvalidBlogDescription_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with { BlogDescription = string.Empty };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var error = result.Value as ErrorModel;

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.Equal("Invalid blog description.", error.Message);
    }

    [Fact]
    public async Task TestHandle_WhenInvalidSeoMetaTitle_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with { SeoMetaTitle = new string('a', 61) }; // Too long title

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var error = result.Value as ErrorModel;

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.Equal("Invalid SEO title.", error.Message);
    }

    [Fact]
    public async Task TestHandle_WhenInvalidSeoMetaDescription_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with { SeoMetaDescription = new string('a', 161) }; // Too long description

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var error = result.Value as ErrorModel;

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.Equal("Invalid SEO description.", error.Message);
    }

    [Fact]
    public async Task TestHandle_WhenInvalidBlogUrl_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with { BlogUrl = string.Empty };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var error = result.Value as ErrorModel;

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.Equal("Invalid blog URL.", error.Message);
    }

    [Fact]
    public async Task TestHandle_WhenInvalidSocialNetworkName_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with
        {
            Socials = [new() { Name = (SocialNetworkName)999, Url = "https://invalid.com" }]
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var error = result.Value as ErrorModel;

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.Equal("Invalid social network name.", error.Message);
    }
}