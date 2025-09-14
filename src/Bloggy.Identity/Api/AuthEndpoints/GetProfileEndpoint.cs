using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Auth;
using Bloggy.Identity.Application.Types.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.AuthEndpoints;

public class GetProfileEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for getting user profile
        app.MapGroup(RouteConstants.AuthBaseRoute)
            .MapGet("profile", async (IOperationService operations,
                [FromHeader] string requestedBy) =>
            {
                // Operation
                var operationResult = await operations.GetUserProfile
                    .ExecuteAsync(new GetUserProfileCommand(RequestedById: requestedBy));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new GetUserProfileResponse(
                            UserId: operationResult.Value.UserId,
                            Email: operationResult.Value.Email,
                            FirstName: operationResult.Value.FirstName,
                            LastName: operationResult.Value.LastName,
                            FullName: operationResult.Value.FullName,
                            Role: operationResult.Value.Role,
                            CreatedAt: operationResult.Value.CreatedAt,
                            UpdatedAt: operationResult.Value.UpdatedAt
                        )),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(RouteConstants.AuthEndpointGroupTag);
    }
}

public record GetUserProfileResponse(
    string UserId,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    Role Role,
    DateTime CreatedAt,
    DateTime UpdatedAt
);