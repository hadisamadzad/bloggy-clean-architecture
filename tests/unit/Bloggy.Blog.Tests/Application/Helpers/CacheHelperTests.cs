using System;
using Bloggy.Blog.Application.Helpers;
using Xunit;

namespace Bloggy.Blog.Tests.Application.Helpers;

public class CacheHelperTests
{
    [Fact]
    public void TestTtlFromSeconds_WhenValueProvided_ShouldReturnCorrectExpiration()
    {
        // Act
        var options = CacheHelper.TtlFromSeconds(30);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(30), options.AbsoluteExpirationRelativeToNow);
    }

    [Fact]
    public void TestTtlFromMinutes_WhenValueProvided_ShouldReturnCorrectExpiration()
    {
        var options = CacheHelper.TtlFromMinutes(5);

        Assert.Equal(TimeSpan.FromMinutes(5), options.AbsoluteExpirationRelativeToNow);
    }

    [Fact]
    public void TestTtlFromHours_WhenValueProvided_ShouldReturnCorrectExpiration()
    {
        var options = CacheHelper.TtlFromHours(2);

        Assert.Equal(TimeSpan.FromHours(2), options.AbsoluteExpirationRelativeToNow);
    }

    [Fact]
    public void TestTtlFromDays_WhenValueProvided_ShouldReturnCorrectExpiration()
    {
        var options = CacheHelper.TtlFromDays(1);

        Assert.Equal(TimeSpan.FromDays(1), options.AbsoluteExpirationRelativeToNow);
    }
}