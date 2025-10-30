using Bloggy.Core.Interfaces;

namespace Bloggy.Blog.Api;

public class DevEndpoints : IEndpoint
{
    // Endpoints
    public void MapEndpoints(WebApplication app)
    {
        // Test redis
        app.MapGroup(Routes.DevBaseRoute)
            .MapPost("redis", async (ICacheService cache) =>
            {
                _ = await cache.SetAsync("test", "test", TimeSpan.FromMinutes(1));
                _ = await cache.GetAsync<string>("test");

                return Results.Ok("Redis works as expected!");
            })
            .WithTags(Routes.DevEndpointGroupTag);
    }
}