using Common.Extensions;
using Xunit;

namespace Common.Tests.Extensions;

public class EnumExtensionsTests
{
    private enum Sample
    {
        Short,
        MediumLength,
        VeryLongEnumName
    }

    [Fact]
    public void TestGetMaxLength_ReturnsCorrectMaxLength()
    {
        // Arrange
        var value = Sample.VeryLongEnumName;

        // Act
        int maxLength = value.GetMaxLength();

        // Assert
        Assert.Equal("VeryLongEnumName".Length, maxLength);
    }

    [Fact]
    public void TestGetMaxLength_ReturnsCorrectMaxLength2()
    {
        // Act
        var maxLength = typeof(Sample).GetEnumMaxLength();

        // Assert
        Assert.Equal("VeryLongEnumName".Length, maxLength);
    }
}