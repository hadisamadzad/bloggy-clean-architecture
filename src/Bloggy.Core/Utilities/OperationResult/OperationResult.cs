namespace Bloggy.Core.Utilities.OperationResult;

public record OperationResult<TResult>(
    OperationStatus Status,
    TResult? Value = default,
    OperationError? Error = null,
    Dictionary<string, string>? Metadata = null)
{
    public readonly OperationStatus Status = Status;
    public readonly TResult? Value = Value;
    public readonly OperationError? Error = Error;
    public readonly Dictionary<string, string>? Metadata = Metadata;

    public bool Succeeded => Status is OperationStatus.Completed or OperationStatus.NoOperation;

    // Factory methods
    public static OperationResult<TResult> Success(TResult value) => new(OperationStatus.Completed, value);
    public static OperationResult<NoResult> Success() => new(OperationStatus.Completed, new NoResult());

    public static OperationResult<TResult> NoOperation(TResult value) => new(OperationStatus.NoOperation, default);
    public static OperationResult<NoResult> NoOperation() => new(OperationStatus.NoOperation, new NoResult());

    public static OperationResult<TResult> ValidationFailure(string[] messages) =>
        new(OperationStatus.Invalid, Error: OperationError.Validation(messages));

    public static OperationResult<TResult> AuthorizationFailure(string message) =>
        new(OperationStatus.Unauthorized, Error: OperationError.Authorization(message));

    public static OperationResult<TResult> Failure(string message) =>
        new(OperationStatus.Failed, Error: OperationError.Unexpected(message));
}

public enum OperationStatus
{
    Completed = 1,
    NoOperation,

    Invalid,
    Unauthorized,
    Failed
}