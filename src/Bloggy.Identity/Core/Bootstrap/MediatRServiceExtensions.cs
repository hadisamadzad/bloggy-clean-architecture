using Bloggy.Identity.Application.Operations.Auth;

namespace Bloggy.Identity.Core.Bootstrap;

public static class MediatRServiceExtensions
{
    public static IServiceCollection AddConfiguredMediatR(this IServiceCollection services)
    {
        // Handlers
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<LoginCommand>());

        return services;
    }
}
