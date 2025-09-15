using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Users;
using Bloggy.Identity.Application.Types.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.UserEndpoints;

public class UpdateUserStatusEndpoint : IEndpoint
{
    public record UpdateUserStatusRequest(UserState State);

    public void MapEndpoints(WebApplication app)
    {
        app.MapGroup(Routes.UserBaseRoute)
            .WithSummary("Update User Status")
            .MapPatch("{userId}/status", async (IOperationService operations,
                [FromRoute] string userId,
                [FromHeader] string requestedBy,
                [FromBody] UpdateUserStatusRequest request) =>
            {
                // Operation
                var operationResult = await operations.UpdateUserState.ExecuteAsync(
                    new UpdateUserStatusCommand(
                        requestedBy,
                        userId,
                        request.State
                    ));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.Unauthorized(),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.UserEndpointGroupTag)
            .WithDescription("Updates the status of a user (e.g., Active, Inactive, Suspended).");
    }
}
