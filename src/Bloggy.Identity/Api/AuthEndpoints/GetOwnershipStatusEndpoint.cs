using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Auth;

namespace Bloggy.Identity.Api.AuthEndpoints;

public class GetOwnershipStatusEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for checking if ownership is done
        app.MapGroup(RouteConstants.AuthBaseRoute)
            .MapGet("ownership-check", async (IOperationService operations
                ) =>
            {
                // Operation
                var operationResult = await operations.GetOwnershipStatus
                    .ExecuteAsync(new GetOwnershipStatusCommand());

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new GetOwnershipStatusResponse(IsAlreadyOwned: operationResult.Value)),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(RouteConstants.AuthEndpointGroupTag)
            .WithSummary("Checks whether the service ownership stage is completed.")
            .WithDescription("Returns a boolean indicating whether the one-off " +
                "ownership process is completed or not.");
    }
}

public record GetOwnershipStatusResponse(bool IsAlreadyOwned);