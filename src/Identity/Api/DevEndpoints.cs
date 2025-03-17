using Common.Interfaces;
using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api;

public class DevEndpoints : IEndpoint
{
    const string Route = "api/dev/";
    const string Tag = "Dev";

    // Endpoints
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup(Route).WithTags(Tag);

        // Test logger
        group.MapGet("logger/test", (
            ILogger<object> logger,
            [FromQuery] string message) =>
            {
                logger.LogInformation("Hey, we have got a log: {message}", message);
                return Results.Ok(true);
            });

        // Test email service
        group.MapGet("email", async (
            IEmailService emailService) =>
            {
                var parameters = new Dictionary<string, string>
                {
                    { "Link", "https://hadisamadzad.com" }
                };

                _ = await emailService
                    .SendEmailByTemplateIdAsync(1, ["h.samadzad@gmail.com"], parameters);

                return Results.Ok("Email sent");
            });

        // Test redis
        group.MapGet("redis", async (
            ICacheService cache) =>
            {
                _ = await cache.SetAsync("test", "test", TimeSpan.FromMinutes(1));
                _ = await cache.GetAsync<string>("test");

                return Results.Ok("Redis works as expected!");
            });


    }
}