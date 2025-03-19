using Common.Helpers;
using Xunit;

namespace Common.Tests.Helpers;

public class Base64HelperTests
{
    [Theory]
    [InlineData("a+b/c=", "a-b_c=")]
    [InlineData("hello+world/", "hello-world_")]
    public void TestConvertBase64ToBase64Url_ShouldConvertSuccessfully(
        string base64, string expected)
    {
        // Act
        var result = Base64Helper.ConvertBase64ToBase64Url(base64);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("a-b_c=", "a+b/c=")]
    [InlineData("hello-world_", "hello+world/")]
    public void TestConvertBase64UrlToBase64_ShouldConvertSuccessfully(
        string base64Url, string expected)
    {
        // Act
        var result = Base64Helper.ConvertBase64UrlToBase64(base64Url);

        // Assert
        Assert.Equal(expected, result);
    }
}