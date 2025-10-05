using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bloggy.Core.Persistence.MongoDB;

public static class MongoDbServiceExtensions
{
    public static IServiceCollection AddConfiguredMongoDB(this IServiceCollection services,
        IConfiguration configuration)
    {
        var config = configuration.GetSection(MongoDBConfig.Key).Get<MongoDBConfig>();

        services.AddSingleton(MongoDBContext.Connect(config));

        return services;
    }
}