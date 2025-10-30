using Bloggy.Core.Persistence.Redis;

namespace Bloggy.Blog.Core.Bootstrap;

public static class RedisServiceExtensions
{
    public static IServiceCollection AddConfiguredRedisCache(this IServiceCollection services,
        IConfiguration configuration)
    {
        var config = configuration.GetSection(RedisConfig.Key).Get<RedisConfig>();

        // Distributed caching
        services.AddRedisCache("blog", config!);

        return services;
    }
}