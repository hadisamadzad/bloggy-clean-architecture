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

public class GetArticleByIdHandlerTests
{
    private readonly IRepositoryManager _repository;
    private readonly GetArticleByIdHandler _handler;

    public GetArticleByIdHandlerTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _handler = new GetArticleByIdHandler(_repository);
    }

    [Fact]
    public async Task TestHandle_WhenArticleIdIsInvalid_ShouldReturnInvalid()
    {
        // Arrange
        var request = new GetArticleByIdQuery("");

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(OperationStatus.Invalid, result.Status);
        await _repository.Articles.DidNotReceive().GetByIdAsync(Arg.Any<string>());
    }

    [Fact]
    public async Task TestHandle_WhenArticleDoesNotExist_ShouldReturnUnprocessable()
    {
        // Arrange
        var request = new GetArticleByIdQuery("non-existent-id");
        _repository.Articles.GetByIdAsync(request.ArticleId).Returns((ArticleEntity)null!);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(OperationStatus.Failed, result.Status);
    }

    [Fact]
    public async Task TestHandle_WhenArticleExists_ShouldReturnSuccess()
    {
        // Arrange
        var request = new GetArticleByIdQuery("article-1");
        var tagIds = new[] { "tag-1" };
        _repository.Articles.GetByIdAsync(request.ArticleId)
            .Returns(
                new ArticleEntity
                {
                    Id = "article-1",
                    AuthorId = UidHelper.GenerateNewId("user"),
                    TagIds = tagIds
                });

        _repository.Tags.GetByIdsAsync(tagIds)
            .Returns([
                new TagEntity{
                    Id = "tag-1",
                    Name = "tag-1",
                    Slug = "tag-1"
                }
            ]);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal(OperationStatus.Completed, result.Status);
    }
}