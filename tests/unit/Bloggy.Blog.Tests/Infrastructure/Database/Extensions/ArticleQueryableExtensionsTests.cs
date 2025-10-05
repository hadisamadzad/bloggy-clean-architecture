using System;
using System.Collections.Generic;
using System.Linq;
using Blog.Infrastructure.Database.Extensions;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.Types.Models.Articles;
using Xunit;

namespace Bloggy.Blog.Tests.Infrastructure.Database.Extensions;

public class ArticleQueryableExtensionsTests
{
    private readonly List<ArticleEntity> _articles =
    [
        new ArticleEntity
        {
            Id = "1",
            AuthorId = string.Empty,
            Title = "First Blog",
            Summary = "Summary A",
            Content = "Content A",
            TagIds = ["tag1"],
            Status = ArticleStatus.Published,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            Likes = 10
        },
        new ArticleEntity
        {
            Id = "2",
            AuthorId = string.Empty,
            Title = "Second Blog",
            Summary = "Summary B",
            Content = "Content B",
            TagIds = new List<string> { "tag2" },
            Status = ArticleStatus.Draft,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Likes = 5
        }
    ];

    [Fact]
    public void TestApplyFilter_WhenKeywordProvided_ShouldFilterByKeyword()
    {
        var filter = new ArticleFilter { Keyword = "First" };
        var query = _articles.AsQueryable().ApplyFilter(filter);

        Assert.Single(query);
        Assert.Equal("1", query.First().Id);
    }

    [Fact]
    public void TestApplyFilter_WhenTagIdProvided_ShouldFilterByTag()
    {
        var filter = new ArticleFilter { TagIds = new List<string> { "tag2" } };
        var query = _articles.AsQueryable().ApplyFilter(filter);

        Assert.Single(query);
        Assert.Equal("2", query.First().Id);
    }

    [Fact]
    public void TestApplyFilter_WhenStatusProvided_ShouldFilterByStatus()
    {
        var filter = new ArticleFilter { Statuses = new List<ArticleStatus> { ArticleStatus.Published } };
        var query = _articles.AsQueryable().ApplyFilter(filter);

        Assert.Single(query);
        Assert.Equal("1", query.First().Id);
    }

    [Fact]
    public void TestApplySort_WhenSortByLikesMost_ShouldSortByLikesDescending()
    {
        var query = _articles.AsQueryable().ApplySort(ArticleSortBy.LikesMost).ToList();

        Assert.Equal("1", query[0].Id);
        Assert.Equal("2", query[1].Id);
    }
}