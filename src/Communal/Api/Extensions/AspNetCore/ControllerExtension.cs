using Communal.Application.Infrastructure.Errors;
using Communal.Application.Infrastructure.Operations;
using Microsoft.AspNetCore.Mvc;

namespace Communal.Api.Extensions.AspNetCore;

public static class ControllerExtension
{
    public static IActionResult ReturnResponse(this ControllerBase controller, OperationResult operation)
    {
        object response = operation.Value;
        if (response is ErrorModel errorModel)
            response = new ErrorResponse(errorModel);

        return operation.Status switch
        {
            OperationStatus.Completed => controller.Ok(response),
            OperationStatus.Ignored => controller.Ok(response),
            OperationStatus.ValidationFailed => controller.BadRequest(response),
            OperationStatus.NotFound => controller.NotFound(response),
            OperationStatus.Unauthorized => controller.UnprocessableEntity(response),
            OperationStatus.Unprocessable => controller.UnprocessableEntity(response),
            _ => controller.UnprocessableEntity(response)
        };
    }
}
