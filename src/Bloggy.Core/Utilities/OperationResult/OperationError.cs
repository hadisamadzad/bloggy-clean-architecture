namespace Bloggy.Core.Utilities.OperationResult;

public record OperationError(OperationErrorType Type, string[] Messages)
{
    public static OperationError Validation(string[] messages) => new(OperationErrorType.ValidationError, messages);
    public static OperationError Authorization(string message) => new(OperationErrorType.AuthorizationError, [message]);
    public static OperationError Unexpected(string message) => new(OperationErrorType.UnexpectedError, [message]);
}

public enum OperationErrorType
{
    NotSpecified = 1,
    ValidationError,
    AuthorizationError,
    UnexpectedError
}