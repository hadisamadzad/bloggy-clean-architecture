using System;
using Common.Helpers;
using Xunit;

namespace Common.Tests.Helpers;

public class RandomGeneratorTests
{
    [Theory]
    [InlineData(10, AllowedCharacters.Numeric)]
    [InlineData(15, AllowedCharacters.Alphanumeric)]
    [InlineData(5, AllowedCharacters.AlphanumericReadable, "usr-")]
    public void TestGenerateString_WhenValidInput_ShouldReturnCorrectLength(
        int length, string allowedCharacters, string? prefix = null)
    {
        // Act
        var result = RandomGenerator.GenerateString(length, allowedCharacters, prefix);

        // Assert
        Assert.Equal(length + (prefix?.Length ?? 0), result.Length);
        Assert.StartsWith(prefix ?? string.Empty, result);
    }

    [Theory]
    [InlineData(100, 0)]
    [InlineData(50, 10)]
    public void TestGenerateNumber_WhenValidRange_ShouldReturnInRange(int max, int min)
    {
        // Act
        var number = RandomGenerator.GenerateNumber(max, min);

        // Assert
        Assert.InRange(number, min, max - 1);
    }

    [Fact]
    public void TestGenerateNumber_WhenInvalidRange_ShouldThrowException()
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RandomGenerator.GenerateNumber(-1));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RandomGenerator.GenerateNumber(5, 10));
    }
}