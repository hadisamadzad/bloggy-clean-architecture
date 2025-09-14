using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.PasswordReset;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.PasswordResetEndpoints;

public class PasswordResetEndpoints : IEndpoint
{
    // Models
    public record SendPasswordResetEmailRequest(string Email);
    public record GetPasswordResetEmailResponse(string Email);
    public record ResetPasswordRequest(string Token, string NewPassword);

    // Endpoints
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for sending password reset email
        app.MapGroup(Routes.AuthBaseRoute)
            .MapPost("password-reset", async (IOperationService operations,
            [FromBody] SendPasswordResetEmailRequest request) =>
            {
                // Operation
                var operationResult = await operations.SendPasswordResetEmail.ExecuteAsync(
                    new SendPasswordResetEmailCommand(request.Email));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.PasswordResetEndpointGroupTag);

        // Endpoint for getting password reset info
        app.MapGroup(Routes.AuthBaseRoute)
            .MapGet("password-reset", async (IOperationService operations,
            [FromQuery] string token) =>
            {
                // Operation
                var operationResult = await operations.GetPasswordResetEmail
                    .ExecuteAsync(new GetPasswordResetEmailCommand(token));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new GetPasswordResetEmailResponse(
                            Email: operationResult.Value
                        )),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.PasswordResetEndpointGroupTag);

        // Endpoint for resetting user password
        app.MapGroup(Routes.AuthBaseRoute)
            .MapPatch("password-reset", async (IOperationService operations,
            [FromBody] ResetPasswordRequest request) =>
            {
                // Operation
                var operationResult = await operations.ResetPassword.ExecuteAsync(
                    new ResetPasswordCommand(request.Token, request.NewPassword));

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
            .WithTags(Routes.PasswordResetEndpointGroupTag);
    }
}