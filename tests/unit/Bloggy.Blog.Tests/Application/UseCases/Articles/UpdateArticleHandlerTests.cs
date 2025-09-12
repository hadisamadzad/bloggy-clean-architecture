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

public class UpdateArticleHandlerTests
{
    private readonly IRepositoryManager _repository;
    private readonly UpdateArticleHandler _handler;

    public UpdateArticleHandlerTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _handler = new UpdateArticleHandler(_repository);
    }

    [Fact]
    public async Task TestHandle_WhenValidationFails_ShouldReturnInvalid()
    {
        // Arrange
        var request = new UpdateArticleCommand { ArticleId = "", Title = "" }; // Missing required fields

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Invalid, result.Status);
    }

    [Fact]
    public async Task TestHandle_WhenSlugAlreadyExists_ShouldReturnUnprocessable()
    {
        // Arrange
        var request = new UpdateArticleCommand
        {
            ArticleId = "article-1",
            Title = "Test",
            Slug = "existing-slug",
            TagIds = ["tag-1"]
        };

        _repository.Articles.GetBySlugAsync(request.Slug).Returns(
            new ArticleEntity
            {
                Id = string.Empty,
                AuthorId = string.Empty
            });

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(OperationStatus.Failed, result.Status);
    }

    [Fact]
    public async Task TestHandle_WhenValidRequest_ShouldUpdateArticle()
    {
        // Arrange
        var request = new UpdateArticleCommand
        {
            ArticleId = "article-1",
            Title = "Updated Title",
            Slug = "updated-title",
            TagIds = ["tag-1"]
        };

        var article = new ArticleEntity
        {
            Id = request.ArticleId,
            AuthorId = UidHelper.GenerateNewId("user")
        };

        _repository.Articles.GetBySlugAsync(request.Slug).Returns((ArticleEntity)null!);
        _repository.Articles.GetByIdAsync(request.ArticleId).Returns(article);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        await _repository.Articles.Received(1).UpdateAsync(article);
        Assert.Equal(request.Title, article.Title);
        Assert.Equal(request.Slug.ToLower(), article.Slug);
    }
}