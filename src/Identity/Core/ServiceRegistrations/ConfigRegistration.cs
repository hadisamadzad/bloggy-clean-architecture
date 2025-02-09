﻿using Identity.Application.Helpers;
using Identity.Application.Types.Configs;

namespace Identity.Core.ServiceRegistrations;

public static class ConfigRegistration
{
    public static IServiceCollection AddCustomConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        // Jwt helper static config
        var jwtConfig = configuration.GetSection(AuthTokenConfig.Key).Get<AuthTokenConfig>();
        JwtHelper.Initialize(jwtConfig);

        // User helper static lockout config
        UserHelper.LockoutConfig = configuration.GetSection(LockoutConfig.Key).Get<LockoutConfig>();

        //services.Configure<RedisCacheConfig>(configuration.GetSection(RedisCacheConfig.Key));
        services.Configure<ActivationConfig>(configuration.GetSection(ActivationConfig.Key));
        services.Configure<PasswordResetConfig>(configuration.GetSection(PasswordResetConfig.Key));
        services.Configure<BrevoConfig>(configuration.GetSection(BrevoConfig.Key));

        return services;
    }
}
