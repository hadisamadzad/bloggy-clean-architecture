using Bloggy.Gateway.Core.DelegatingHandlers;
using Ocelot.DependencyInjection;

namespace Bloggy.Gateway.Core.DependencyInjection;

public static class OcelotInjection
{
    public static IServiceCollection AddConfiguredOcelot(this IServiceCollection services)
    {
        services
            .AddOcelot()
            .AddDelegatingHandler<GlobalDelegatingHandler>(global: true);

        return services;
    }
}