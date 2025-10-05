using System.Threading;
using System.Threading.Tasks;
using Bloggy.Blog.Application.Constants;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Settings;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.Types.Models.Settings;
using Bloggy.Core.Utilities.OperationResult;
using NSubstitute;
using Xunit;

namespace Bloggy.Blog.Tests.Application.Operations.Settings;

public class GetBlogSettingsOperationTests
{
    private readonly IRepositoryManager _repository;
    private readonly GetBlogSettingsOperation _operation;

    public GetBlogSettingsOperationTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _operation = new GetBlogSettingsOperation(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSettingsFound_ShouldReturnSuccess()
    {
        // Arrange
        var settings = new SettingEntity
        {
            Id = "settings-1",
            BlogTitle = "Test Title",
            BlogSubtitle = "Test Subtitle",
            BlogPageTitle = "Test Page Title"
        };
        _repository.Settings.GetBlogSettingAsync().Returns(settings);

        // Act
        var result = await _operation.ExecuteAsync(new GetBlogSettingsCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.IsType<SettingModel>(result.Value);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSettingsNotFound_ShouldReturnUnprocessable()
    {
        // Arrange
        _repository.Settings.GetBlogSettingAsync().Returns((SettingEntity)null!);

        // Act
        var result = await _operation.ExecuteAsync(new GetBlogSettingsCommand(), CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.NotFound, result.Status);
        Assert.NotNull(result.Error);
    }
}