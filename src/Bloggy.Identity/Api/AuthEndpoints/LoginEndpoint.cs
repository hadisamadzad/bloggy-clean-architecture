using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.AuthEndpoints;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for logging in
        app.MapGroup(Routes.AuthBaseRoute)
            .WithSummary("Login endpoint")
            .MapPost("login", async (IOperationService operations,
                [FromBody] LoginRequest request) =>
            {
                // Operation
                var operationResult = await operations.Login.ExecuteAsync(new LoginCommand
                (
                    Email: request.Email.Trim(),
                    Password: request.Password.Trim()
                ));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new LoginResponse(
                            Email: operationResult.Value!.Email,
                            FullName: operationResult.Value.FullName,
                            AccessToken: operationResult.Value.AccessToken,
                            RefreshToken: operationResult.Value.RefreshToken
                        )),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    OperationStatus.Failed => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.AuthEndpointGroupTag)
            .WithDescription("Authenticates a user and returns access and refresh tokens.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record LoginRequest(string Email, string Password);
public record LoginResponse(
    string Email,
    string FullName,
    string AccessToken,
    string RefreshToken
);