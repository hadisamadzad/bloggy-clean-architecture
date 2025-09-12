using System.Threading;
using System.Threading.Tasks;
using Bloggy.Blog.Application.Constants;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.UseCases.Settings;
using Bloggy.Core.Utilities.OperationResult;
using NSubstitute;
using Xunit;

namespace Bloggy.Blog.Tests.Application.UseCases.Settings;

public class GetBlogSettingsHandlerTests
{
    private readonly IRepositoryManager _repository;
    private readonly GetBlogSettingsHandler _handler;

    public GetBlogSettingsHandlerTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _handler = new GetBlogSettingsHandler(_repository);
    }

    [Fact]
    public async Task TestHandle_WhenSettingsFound_ShouldReturnSuccess()
    {
        // Arrange
        var settings = new SettingEntity { Id = "settings-1", BlogTitle = string.Empty };
        _repository.Settings.GetBlogSettingAsync().Returns(settings);

        // Act
        var result = await _handler.Handle(new GetBlogSettingsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(settings, result.Value);
    }

    [Fact]
    public async Task TestHandle_WhenSettingsNotFound_ShouldReturnUnprocessable()
    {
        // Arrange
        _repository.Settings.GetBlogSettingAsync().Returns((SettingEntity)null!);

        // Act
        var result = await _handler.Handle(new GetBlogSettingsQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Failed, result.Status);
        Assert.Equal(Errors.SettingsNotFound, result.Value);
    }
}