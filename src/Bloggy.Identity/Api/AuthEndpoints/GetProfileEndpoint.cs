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
        app.MapGroup(Routes.AuthBaseRoute)
            .WithSummary("Gets the current user's profile")
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
                            UserId: operationResult.Value!.UserId,
                            Email: operationResult.Value.Email,
                            IsEmailConfirmed: operationResult.Value.IsEmailConfirmed,
                            FirstName: operationResult.Value.FirstName,
                            LastName: operationResult.Value.LastName,
                            FullName: operationResult.Value.FullName,
                            Role: operationResult.Value.Role,
                            Status: operationResult.Value.State,
                            LastLoginDate: operationResult.Value.LastLoginDate,
                            CreatedAt: operationResult.Value.CreatedAt,
                            UpdatedAt: operationResult.Value.UpdatedAt
                        )),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.AuthEndpointGroupTag)
            .WithDescription("Returns the profile information for the authenticated user.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record GetUserProfileResponse(
    string UserId,
    string Email,
    bool IsEmailConfirmed,
    string? FirstName,
    string? LastName,
    string FullName,
    Role Role,
    UserState Status,
    DateTime? LastLoginDate,
    DateTime CreatedAt,
    DateTime UpdatedAt
);