using System;
using Blog.Application.Helpers;
using Xunit;

namespace Blog.Tests.Application.Helpers;

public class SlugHelperTests
{
    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("C# & .NET @2024", "csharp-and-net-at2024")]
    [InlineData("A+B+C", "aplusbplusc")]
    public void TestGenerateSlug_WhenValidInput_ShouldGenerateCorrectSlug(
        string input, string expected)
    {
        // Act
        var result = SlugHelper.GenerateSlug(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestGenerateSlug_WhenInputIsNullOrWhitespace_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => SlugHelper.GenerateSlug(""));
        Assert.Throws<ArgumentException>(() => SlugHelper.GenerateSlug("   "));
    }

    [Theory]
    [InlineData("hello-world", true)]
    [InlineData("hello--world", false)]
    [InlineData("-hello-world", false)]
    [InlineData("hello-world-", false)]
    [InlineData("hello@world", false)]
    public void TestIsValidSlug_WhenChecked_ShouldReturnExpectedResult(
        string input, bool expected)
    {
        // Act
        var result = SlugHelper.IsValidSlug(input);

        // Assert
        Assert.Equal(expected, result);
    }
}