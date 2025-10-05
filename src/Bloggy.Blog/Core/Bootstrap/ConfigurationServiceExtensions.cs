namespace Blog.Core.Bootstrap;

public static class ConfigurationServiceExtensions
{
    public static IServiceCollection AddCustomConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}
