using Common.Helpers;
using Xunit;

namespace Common.Tests.Helpers;

public class UidHelperTests
{
    [Fact]
    public void TestGenerateNewId_WhenNoPrefix_ShouldReturnUlid()
    {
        // Act
        var result = UidHelper.GenerateNewId();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result));
        Assert.Matches(@"^[0-9a-z]{26}$", result); // ulid is 26 characters lowercased
    }

    [Fact]
    public void TestGenerateNewId_WhenPrefixProvided_ShouldReturnPrefixedUlid()
    {
        // Arrange
        string prefix = "user";

        // Act
        var result = UidHelper.GenerateNewId(prefix);

        // Assert
        Assert.StartsWith($"{prefix}-", result);
        Assert.Equal($"{prefix}-".Length + 26, result.Length);
    }

    [Fact]
    public void TestGenerateNewId_WhenPrefixIsWhitespace_ShouldIgnorePrefix()
    {
        // Act
        var result = UidHelper.GenerateNewId("  ");

        // Assert
        Assert.False(result.StartsWith("  "));
        Assert.Matches(@"^[0-9a-z]{26}$", result);
    }
}