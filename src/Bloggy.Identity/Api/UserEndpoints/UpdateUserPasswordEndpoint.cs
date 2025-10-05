using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Users;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.UserEndpoints;

public class UpdateUserPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGroup(Routes.UserBaseRoute)
            .WithSummary("Update User Password")
            .MapPatch("{userId}/password", async (IOperationService operations,
                [FromRoute] string userId,
                [FromHeader] string requestedBy,
                [FromBody] UpdateUserPasswordRequest request) =>
            {
                // Operation
                var operationResult = await operations.UpdateUserPassword.ExecuteAsync(
                    new UpdateUserPasswordCommand(
                        requestedBy,
                        userId,
                        request.CurrentPassword,
                        request.NewPassword));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    OperationStatus.Failed => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.UserEndpointGroupTag)
            .WithDescription("Update a user's password. Requires the current password and the new password.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record UpdateUserPasswordRequest(string CurrentPassword, string NewPassword);