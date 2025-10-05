using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Articles;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.Pagination;
using NSubstitute;
using Xunit;

namespace Bloggy.Blog.Tests.Application.Operations.Articles;

public class GetArticlesByFilterOperationTests
{
    private readonly IRepositoryManager _repository;
    private readonly GetArticlesByFilterOperation _operation;

    public GetArticlesByFilterOperationTests()
    {
        _repository = Substitute.For<IRepositoryManager>();
        _operation = new GetArticlesByFilterOperation(_repository);
    }

    [Fact]
    public async Task ExecuteAsync_WhenFilterIsNull_ShouldDefaultToPaginationAndReturnResult()
    {
        // Arrange
        var query = new GetArticlesByFilterCommand(null!);

        _repository.Articles.GetByFilterAsync(Arg.Any<ArticleFilter>())
            .Returns([new() { Id = "1", AuthorId = UidHelper.GenerateNewId("user") }]);

        _repository.Articles.CountByFilterAsync(Arg.Any<ArticleFilter>())
            .Returns(1);

        // Act
        var result = await _operation.ExecuteAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.IsType<PaginatedList<ArticleModel>>(result.Value);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoArticlesFound_ShouldReturnEmptyPaginatedList()
    {
        // Arrange
        var query = new GetArticlesByFilterCommand(new ArticleFilter { Page = 1, PageSize = 10 });

        _repository.Articles.GetByFilterAsync(Arg.Any<ArticleFilter>())
            .Returns((List<ArticleEntity>)null!);
        _repository.Articles.CountByFilterAsync(Arg.Any<ArticleFilter>()).Returns(0);

        // Act
        var result = await _operation.ExecuteAsync(query, CancellationToken.None);

        // Assert
        var paginatedList = Assert.IsType<PaginatedList<ArticleModel>>(result.Value);
        Assert.Empty(paginatedList.Results);
        Assert.Equal(0, paginatedList.TotalCount);
    }
}