using Bloggy.Core.Interfaces;

namespace Bloggy.Identity.Api.AuthEndpoints;

public class LogoutEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for logging out
        app.MapGroup(Routes.AuthBaseRoute)
            .WithSummary("Logout endpoint")
            .MapPost("logout", (HttpContext context) =>
            {
                // Clear the refresh token cookie
                // Only need Path, Domain, and SameSite for proper cookie deletion
                context.Response.Cookies.Delete("refreshToken", new CookieOptions
                {
                    Path = "/",
                    Domain = "bloggy.hadisamadzad.com", // Set to null for local development
                    SameSite = SameSiteMode.Strict
                });

                // Return 204 No Content - logout successful, no response body needed
                return Results.NoContent();
            })
            .WithTags(Routes.AuthEndpointGroupTag)
            .WithDescription("Logs out the user by clearing the refresh token cookie.")
            .Produces(StatusCodes.Status204NoContent);
    }
}