using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Bloggy.Core.Utilities.LockManager;

public static class LockManagerInjection
{
    public static IServiceCollection AddLockManager(this IServiceCollection services,
        string instancePrefix, RedisConfig config)
    {
        var options = new ConfigurationOptions();
        foreach (var connection in config.Connections)
            options.EndPoints.Add(connection);

        services.AddSingleton<ILockManager>(_ =>
            new LockManager(options: options, instancePrefix: instancePrefix));

        return services;
    }
}