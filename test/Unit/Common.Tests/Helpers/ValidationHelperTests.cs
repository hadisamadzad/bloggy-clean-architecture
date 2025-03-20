using Common.Helpers;
using FluentValidation.Results;
using Xunit;

namespace Common.Tests.Helpers;

public class ValidationHelperTests
{
    [Fact]
    public void GetFirstErrorMessage_ShouldReturnFirstErrorMessage_WhenErrorsExist()
    {
        // Arrange
        var validationResult = new ValidationResult(
        [
            new("Field1", "Error message 1"),
            new("Field2", "Error message 2")
        ]);

        // Act
        var result = validationResult.GetFirstErrorMessage();

        // Assert
        Assert.Equal("Error message 1", result);
    }

    [Fact]
    public void GetFirstErrorMessage_ShouldReturnNull_WhenNoErrorsExist()
    {
        // Arrange
        var validationResult = new ValidationResult();

        // Act
        var result = validationResult.GetFirstErrorMessage();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetFirstError_ShouldReturnCustomState_WhenErrorsExist()
    {
        // Arrange
        var validationResult = new ValidationResult(
        [
            new("Field1", "Error message 1") { CustomState = "CustomState1" },
            new("Field2", "Error message 2") { CustomState = "CustomState2" }
        ]);

        // Act
        var result = validationResult.GetFirstError();

        // Assert
        Assert.Equal("CustomState1", result);
    }

    [Fact]
    public void GetFirstError_ShouldReturnNull_WhenNoErrorsExist()
    {
        // Arrange
        var validationResult = new ValidationResult();

        // Act
        var result = validationResult.GetFirstError();

        // Assert
        Assert.Null(result);
    }
}