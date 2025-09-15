using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.PasswordReset;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.PasswordResetEndpoints;

public class SendPasswordResetEmailEndpoint : IEndpoint
{
    public record SendPasswordResetEmailRequest(string Email);

    public void MapEndpoints(WebApplication app)
    {
        app.MapGroup(Routes.AuthBaseRoute)
            .WithSummary("Send Password Reset Email")
            .MapPost("password-reset", async (IOperationService operations,
            [FromBody] SendPasswordResetEmailRequest request) =>
            {
                var operationResult = await operations.SendPasswordResetEmail.ExecuteAsync(
                    new SendPasswordResetEmailCommand(request.Email));

                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.PasswordResetEndpointGroupTag)
            .WithDescription("Sends a password reset email including special token to the user's email address.");
    }
}
