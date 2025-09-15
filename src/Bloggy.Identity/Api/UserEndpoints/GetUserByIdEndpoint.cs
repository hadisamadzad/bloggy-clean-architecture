using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Users;
using Bloggy.Identity.Application.Types.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.UserEndpoints;

public class GetUserByIdEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGroup(Routes.UserBaseRoute)
            .WithSummary("Get User by ID")
            .MapGet("{userId}", async (IOperationService operations,
                [FromHeader] string requestedBy,
                [FromRoute] string userId) =>
            {
                // Operation
                var operationResult = await operations.GetUserById.ExecuteAsync(
                    new GetUserByIdCommand(userId));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new GetUserByIdResponse(
                            UserId: operationResult.Value!.UserId,
                            Email: operationResult.Value.Email,
                            Mobile: operationResult.Value.Mobile,
                            Role: operationResult.Value.Role,
                            FirstName: operationResult.Value.FirstName,
                            LastName: operationResult.Value.LastName,
                            FullName: operationResult.Value.FullName,
                            CreatedAt: operationResult.Value.CreatedAt,
                            UpdatedAt: operationResult.Value.UpdatedAt
                        )),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.UserEndpointGroupTag)
            .WithDescription("Retrieves a user's details by their unique identifier.");
    }
}

public record GetUserByIdResponse(
    string UserId,
    string Email,
    string? Mobile,
    Role Role,
    string? FirstName,
    string? LastName,
    string FullName,
    DateTime CreatedAt,
    DateTime UpdatedAt
);