using Identity.Application.Helpers;
using Identity.Application.Types.Configs;

namespace Identity.Core.Bootstrap;

public static class ConfigurationServiceExtensions
{
    public static IServiceCollection AddCustomConfigurations(this IServiceCollection services,
        IConfiguration configs)
    {
        // Jwt helper static config
        var jwtConfig = configs.GetSection(AuthTokenConfig.Key).Get<AuthTokenConfig>();
        JwtHelper.Initialize(jwtConfig);

        // User helper static lockout config
        UserHelper.LockoutConfig = configs.GetSection(LockoutConfig.Key).Get<LockoutConfig>();
        PasswordResetTokenHelper.SetEncryptionKey(configs["PasswordReset:EncryptionKey"]);

        services.Configure<ActivationConfig>(configs.GetSection(ActivationConfig.Key));

        return services;
    }
}
