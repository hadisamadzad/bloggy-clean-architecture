using Microsoft.AspNetCore.Http;

namespace Common.Utilities.OperationResult;

public static class OperationResultExtensions
{
    public static IResult GetHttpResult(this OperationResult operation)
    {
        object response = operation.Value;

        return operation.Status switch
        {
            OperationStatus.Completed => Results.Ok(response),
            OperationStatus.Ignored => Results.Ok(response),
            OperationStatus.Invalid => Results.BadRequest(response),
            OperationStatus.Unauthorized => Results.UnprocessableEntity(response),
            OperationStatus.Unprocessable => Results.UnprocessableEntity(response),
            _ => Results.UnprocessableEntity(response)
        };
    }
}