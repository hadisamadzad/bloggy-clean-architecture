using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bloggy.Blog.Application.Constants;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Settings;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Utilities;
using Bloggy.Core.Utilities.OperationResult;
using NSubstitute;
using Xunit;

namespace Bloggy.Blog.Tests.Application.Operations.Settings;

public class UpdateBlogSettingsOperationTests
{
    private readonly IRepositoryManager _repository;
    private readonly UpdateBlogSettingsOperation _operation;

    readonly UpdateBlogSettingsCommand ValidCommand = new()
    {
        BlogTitle = "Updated Blog Title",
        BlogSubtitle = "Updated Blog Subtitle",
        PageTitleTemplate = "Updated Page Title Template",
        CopyrightText = "Copyright 2024 Bloggy",
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

    public UpdateBlogSettingsOperationTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _operation = new UpdateBlogSettingsOperation(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSettingsNotFound_ShouldReturnUnprocessable()
    {
        // Arrange
        var command = ValidCommand;

        _repository.Settings.GetBlogSettingAsync().Returns((SettingEntity)null!);

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.NotFound, result.Status);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_WhenValidRequest_ShouldUpdateSettings()
    {
        // Arrange
        var command = new UpdateBlogSettingsCommand
        {
            BlogTitle = "Updated Blog Title",
            BlogSubtitle = "Updated Blog Subtitle",
            PageTitleTemplate = "Updated Page Title Template",
            CopyrightText = "Copyright 2024 Bloggy",
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
            BlogTitle = "Old Title",
            BlogSubtitle = "Old Subtitle",
            PageTitleTemplate = "Old Page Title",
            CopyrightText = "Old Copyright"
        };
        _repository.Settings.GetBlogSettingAsync().Returns(existingSettings);

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(OperationStatus.Completed, result.Status);
        Assert.Equal(command.BlogTitle, existingSettings.BlogTitle);
        Assert.Equal(command.BlogDescription, existingSettings.BlogDescription);
        Assert.Equal(command.SeoMetaTitle, existingSettings.SeoMetaTitle);
        Assert.Equal(command.SeoMetaDescription, existingSettings.SeoMetaDescription);
        Assert.Equal(command.BlogUrl, existingSettings.BlogUrl);
        Assert.Equal(command.BlogLogoUrl, existingSettings.BlogLogoUrl);
        Assert.Equal(command.PageTitleTemplate, existingSettings.PageTitleTemplate);
        Assert.Equal(command.CopyrightText, existingSettings.CopyrightText);
        Assert.Equal(command.Socials.Count, existingSettings.Socials.Count);
        await _repository.Settings.Received(1).UpdateAsync(existingSettings);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvalidBlogTitle_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with { BlogTitle = string.Empty };

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.NotNull(result.Error);
        Assert.Contains("'Blog Title' must not be empty.", result.Error.Messages);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvalidBlogDescription_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with { BlogDescription = string.Empty };

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.NotNull(result.Error);
        Assert.Contains("'Blog Description' must not be empty.", result.Error.Messages);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvalidSeoMetaTitle_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with { SeoMetaTitle = new string('a', 61) }; // Too long title

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.NotNull(result.Error);
        Assert.NotEmpty(result.Error.Messages);
        Assert.Contains(result.Error.Messages, m => m.Contains("Seo Meta Title") && m.Contains("60"));
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvalidSeoMetaDescription_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with { SeoMetaDescription = new string('a', 161) }; // Too long description

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.NotNull(result.Error);
        Assert.NotEmpty(result.Error.Messages);
        Assert.Contains(result.Error.Messages, m => m.Contains("Seo Meta Description") && m.Contains("160"));
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvalidBlogUrl_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with { BlogUrl = string.Empty };

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.NotNull(result.Error);
        Assert.Contains("'Blog Url' must not be empty.", result.Error.Messages);
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvalidSocialNetworkName_ShouldReturnInvalid()
    {
        // Arrange
        var command = ValidCommand with
        {
            Socials = [new() { Name = (SocialNetworkName)999, Url = "https://invalid.com" }]
        };

        // Act
        var result = await _operation.ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
        Assert.NotNull(result.Error);
        Assert.Contains("The specified condition was not met for 'Socials'.", result.Error.Messages);
    }
}