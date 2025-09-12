using System.Threading;
using System.Threading.Tasks;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.UseCases.Articles;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using NSubstitute;
using Xunit;

namespace Bloggy.Blog.Tests.Application.UseCases.Articles;

public class UpdateArticleStatusHandlerTests
{
    private readonly IRepositoryManager _repository;
    private readonly UpdateArticleStatusHandler _handler;

    public UpdateArticleStatusHandlerTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _handler = new UpdateArticleStatusHandler(_repository);
    }

    [Fact]
    public async Task TestHandle_WhenArticleNotFound_ShouldReturnUnprocessable()
    {
        // Arrange
        var command = new UpdateArticleStatusCommand("non-existent-id", ArticleStatus.Published);
        _repository.Articles.GetByIdAsync(command.ArticleId).Returns((ArticleEntity)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Failed, result.Status);
    }

    [Fact]
    public async Task TestHandle_WhenPublishingDraft_ShouldSetPublishedAt()
    {
        // Arrange
        var article = new ArticleEntity
        {
            Id = "article-1",
            AuthorId = UidHelper.GenerateNewId("user"),
            Status = ArticleStatus.Draft
        };
        _repository.Articles.GetByIdAsync(article.Id).Returns(article);

        var command = new UpdateArticleStatusCommand(article.Id, ArticleStatus.Published);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(ArticleStatus.Published, article.Status);
        Assert.NotNull(article.PublishedAt);
    }

    [Fact]
    public async Task TestHandle_WhenArchivingArticle_ShouldSetArchivedAt()
    {
        // Arrange
        var article = new ArticleEntity
        {
            Id = "article-2",
            AuthorId = UidHelper.GenerateNewId("user"),
            Status = ArticleStatus.Published
        };
        _repository.Articles.GetByIdAsync(article.Id).Returns(article);

        var command = new UpdateArticleStatusCommand(article.Id, ArticleStatus.Archived);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(ArticleStatus.Archived, article.Status);
        Assert.NotNull(article.ArchivedAt);
    }
}