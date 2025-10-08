using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Auth;

namespace Bloggy.Identity.Api.AuthEndpoints;

public class RefreshAccessTokenEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for getting access token by refresh token
        app.MapGroup(Routes.AuthBaseRoute)
            .WithSummary("Gets a new access token using a refresh token")
            .MapPost("refresh", async (IOperationService operations,
                HttpContext context) =>
            {
                // Get refresh token from cookie
                var refreshToken = context.Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                    return Results.BadRequest(new { message = "Refresh token not found" });

                // Operation
                var operationResult = await operations.GetNewAccessToken
                    .ExecuteAsync(new RefreshAccessTokenCommand(RefreshToken: refreshToken));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new RefreshAccessTokenResponse(
                            AccessToken: operationResult.Value!
                        )),

                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.AuthEndpointGroupTag)
            .WithDescription("Returns a new access token if the current refresh token is valid.")
            .Produces<RefreshAccessTokenResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record RefreshAccessTokenResponse(string AccessToken);