using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Users;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.UserEndpoints;

public class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGroup(Routes.UserBaseRoute)
            .WithSummary("Create a new user by admins")
            .MapPost("", async (IOperationService operations,
                [FromHeader] string requestedBy,
                [FromBody] CreateUserRequest request) =>
            {
                var operationResult = await operations.CreateUser.ExecuteAsync(
                    new CreateUserCommand(
                        requestedBy,
                        request.Email,
                        request.Password)
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                    });

                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new CreateUserResponse(UserId: operationResult.Value!)),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    OperationStatus.Failed => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.UserEndpointGroupTag)
            .WithDescription("Creates a new user in the system. This endpoint is intended for use by administrators.");
    }
}

public record CreateUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);
public record CreateUserResponse(string UserId);