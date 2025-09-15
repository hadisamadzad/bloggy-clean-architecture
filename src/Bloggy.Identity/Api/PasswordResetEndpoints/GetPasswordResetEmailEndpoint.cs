using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.PasswordReset;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.PasswordResetEndpoints;

public class GetPasswordResetEmailEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        app.MapGroup(Routes.AuthBaseRoute)
            .MapGet("password-reset", async (IOperationService operations,
                [FromQuery] string token) =>
            {
                var operationResult = await operations.GetPasswordResetEmail
                    .ExecuteAsync(new GetPasswordResetEmailCommand(token));

                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new GetPasswordResetEmailResponse(
                            Email: operationResult.Value
                        )),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.Unauthorized(),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.PasswordResetEndpointGroupTag)
            .WithSummary("Get Password Reset Email by Token")
            .WithDescription("Retrieves the email associated with a valid password reset token.");
    }
}

public record GetPasswordResetEmailResponse(string Email);