using System.Text;
using Bloggy.Gateway.Core.Configs;
using Microsoft.IdentityModel.Tokens;

namespace Bloggy.Gateway.Core.DependencyInjection;

public static class AuthenticationInjection
{
    public static IServiceCollection AddConfiguredAuthentication(this IServiceCollection services,
        IConfiguration configs)
    {
        var config = configs.GetSection(JwtTokenConfig.Key).Get<JwtTokenConfig>();

        services
            .AddAuthentication()
            .AddJwtBearer(Constants.AuthKey, x =>
            {
                x.TokenValidationParameters = new()
                {
                    ValidIssuer = config.Issuer,
                    ValidAudience = config.Audience,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        //.AddGoogle(googleOptions =>
        //{
        //    googleOptions.ClientId = configs["GoogleAuth:ClientId"];
        //    googleOptions.ClientSecret = configs["GoogleAuth:ClientSecret"];
        //});

        return services;
    }
}
