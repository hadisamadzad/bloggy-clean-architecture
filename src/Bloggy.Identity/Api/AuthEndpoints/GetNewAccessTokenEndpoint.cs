using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Operations.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api.AuthEndpoints;

public class GetNewAccessTokenEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for getting access token by refresh token
        app.MapGroup(RouteConstants.AuthBaseRoute)
            .MapGet("access-token", async (IOperationService operations,
                [FromHeader] string refreshToken) =>
            {
                // Operation
                var operationResult = await operations.GetNewAccessToken
                    .ExecuteAsync(new GetNewAccessTokenCommand(RefreshToken: refreshToken));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new GetNewAccessTokenResponse(
                            NewAccessToken: operationResult.Value.AccessToken
                        )),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    OperationStatus.Unauthorized => Results.Unauthorized(),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(RouteConstants.AuthEndpointGroupTag);
    }
}

public record GetNewAccessTokenResponse(string NewAccessToken);