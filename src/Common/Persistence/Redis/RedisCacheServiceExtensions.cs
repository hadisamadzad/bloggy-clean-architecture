using Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Common.Persistence.Redis;

public static class RedisCacheServiceExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services,
        string instancePrefix, RedisConfig config)
    {
        var options = new ConfigurationOptions();
        foreach (var connection in config.Connections)
            options.EndPoints.Add(connection);

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));

        services.AddScoped<ICacheService>(x =>
            new RedisCacheService(x.GetRequiredService<IConnectionMultiplexer>(), instancePrefix));

        return services;
    }
}
