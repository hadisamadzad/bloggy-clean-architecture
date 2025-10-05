using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.AuthEndpoints;

public class CheckUsernameAvailabilityEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for checking username availability
        app.MapGroup(Routes.AuthBaseRoute)
            .WithSummary("Check Username Availability")
            .MapGet("username-check", async (IOperationService operations,
                [FromQuery] string email) =>
            {
                // Operation
                var operationResult = await operations.CheckUsername.ExecuteAsync(new CheckUsernameCommand(email));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new CheckUsernameAvailabilityResponse(
                            IsAvailable: operationResult.Value
                        )),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.AuthEndpointGroupTag)
            .WithDescription("Checks if a username (email) is available.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record CheckUsernameAvailabilityResponse(bool IsAvailable);