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

public class DeleteArticleHandlerTests
{
    private readonly IRepositoryManager _repository;
    private readonly DeleteArticleHandler _handler;

    public DeleteArticleHandlerTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _handler = new DeleteArticleHandler(_repository);
    }

    [Fact]
    public async Task TestHandle_WhenArticleExists_ShouldSoftDeleteAndReturnSuccess()
    {
        // Arrange
        var article = new ArticleEntity
        {
            Id = "article-1",
            AuthorId = UidHelper.GenerateNewId("user"),
            IsDeleted = false
        };
        _repository.Articles.GetByIdAsync("article-1").Returns(article);

        var command = new DeleteArticleCommand("article-1");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.True(article.IsDeleted);
        await _repository.Articles.Received(1).UpdateAsync(article);
    }

    [Fact]
    public async Task TestHandle_WhenArticleDoesNotExist_ShouldReturnUnprocessable()
    {
        // Arrange
        _repository.Articles.GetByIdAsync("non-existent").Returns((ArticleEntity)null!);
        var command = new DeleteArticleCommand("non-existent");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Unprocessable, result.Status);
        await _repository.Articles.DidNotReceive().UpdateAsync(Arg.Any<ArticleEntity>());
    }
}