using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Users;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.UserEndpoints;

public class UpdateUserEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGroup(Routes.UserBaseRoute)
            .WithSummary("Update user details by admins")
            .MapPatch("{userId}", async (IOperationService operations,
                [FromRoute] string userId,
                [FromHeader] string requestedBy,
                [FromBody] UpdateUserRequest request) =>
            {
                // Operation
                var operationResult = await operations.UpdateUser.ExecuteAsync(
                    new UpdateUserCommand(
                        requestedBy,
                        userId,
                        request.FirstName,
                        request.LastName));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.UserEndpointGroupTag)
            .WithDescription("Updates a user's details such as first name and last name. This endpoint is intended for use by administrators.");
    }
}

public record UpdateUserRequest(string FirstName, string LastName);