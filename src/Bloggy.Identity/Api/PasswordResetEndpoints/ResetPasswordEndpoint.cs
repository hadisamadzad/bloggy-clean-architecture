using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.PasswordReset;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.PasswordResetEndpoints;

public class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGroup(Routes.AuthBaseRoute)
            .WithSummary("Reset Password by Password Reset Token")
            .MapPatch("password-reset", async (IOperationService operations,
                [FromBody] ResetPasswordRequest request) =>
            {
                var operationResult = await operations.ResetPassword.ExecuteAsync(
                    new ResetPasswordCommand(request.Token, request.NewPassword));

                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.PasswordResetEndpointGroupTag)
            .WithDescription("Resets the user's password using a valid password reset token.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record ResetPasswordRequest(string Token, string NewPassword);