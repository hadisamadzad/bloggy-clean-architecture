namespace Gateway.Core.DependencyInjection;

public static class CorsInjection
{
    public static IServiceCollection AddConfiguredCors(this IServiceCollection services,
        IConfiguration configs)
    {
        var origins = configs.GetSection("AllowedOrigins").Get<string>().Split(';');

        services.AddCors(options => options
            .AddPolicy(Constants.CorsPolicyName, policy => policy
            .WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader())
        );

        return services;
    }
}