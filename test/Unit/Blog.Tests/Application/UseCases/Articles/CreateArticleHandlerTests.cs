using System.Threading;
using System.Threading.Tasks;
using Blog.Application.Interfaces;
using Blog.Application.Types.Entities;
using Blog.Application.UseCases.Articles;
using Common.Helpers;
using Common.Utilities.OperationResult;
using NSubstitute;
using Xunit;

namespace Blog.Tests.Application.UseCases.Articles;

public class CreateArticleHandlerTests
{
    private readonly IRepositoryManager _repository;
    private readonly CreateArticleHandler _handler;

    public CreateArticleHandlerTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _handler = new CreateArticleHandler(_repository);
    }

    [Fact]
    public async Task TestHandle_WhenValidCommand_ShouldCreateArticle()
    {
        // Arrange
        var command = new CreateArticleCommand
        {
            AuthorId = "author-1",
            Title = "Test Article",
            TagIds = ["tag-1"]
        };

        _repository.Articles.GetBySlugAsync(Arg.Any<string>()).Returns((ArticleEntity)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        await _repository.Articles.Received(1).InsertAsync(Arg.Any<ArticleEntity>());
    }

    [Fact]
    public async Task TestHandle_WhenValidationFails_ShouldReturnInvalid()
    {
        // Arrange
        var command = new CreateArticleCommand
        {
            AuthorId = "",
            Title = "",
            TagIds = []
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task TestHandle_WhenSlugExists_ShouldReturnUnprocessable()
    {
        // Arrange
        var command = new CreateArticleCommand
        {
            AuthorId = "author-1",
            Title = "Test Article",
            Slug = "existing-slug",
            TagIds = ["tag-1"]
        };

        _repository.Articles.GetBySlugAsync(command.Slug)
            .Returns(
                new ArticleEntity
                {
                    Id = "existing-article",
                    AuthorId = UidHelper.GenerateNewId("user")
                });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Failed, result.Status);
    }
}