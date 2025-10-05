using Bloggy.Core.Interfaces;
using Bloggy.Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api;

public class DevEndpoints : IEndpoint
{
    // Endpoints
    public void MapEndpoints(WebApplication app)
    {
        // Test logger
        app.MapGroup(Routes.DevBaseRoute)
            .MapGet("logger/test", (
            ILogger<object> logger,
            [FromQuery] string message) =>
            {
                logger.LogInformation("Hey, we have got a log: {message}", message);
                return Results.Ok(true);
            })
            .WithTags(Routes.DevEndpointGroupTag);

        // Test email service
        app.MapGroup(Routes.DevBaseRoute)
            .MapGet("email", async (
            IEmailService emailService) =>
            {
                var parameters = new Dictionary<string, string>
                {
                    { "Link", "https://hadisamadzad.com" }
                };

                _ = await emailService
                    .SendEmailByTemplateIdAsync(1, ["h.samadzad@gmail.com"], parameters);

                return Results.Ok("Email sent");
            })
            .WithTags(Routes.DevEndpointGroupTag);

        // Test redis
        app.MapGroup(Routes.DevBaseRoute)
            .MapGet("redis", async (
            ICacheService cache) =>
            {
                _ = await cache.SetAsync("test", "test", TimeSpan.FromMinutes(1));
                _ = await cache.GetAsync<string>("test");

                return Results.Ok("Redis works as expected!");
            })
            .WithTags(Routes.DevEndpointGroupTag);
    }
}