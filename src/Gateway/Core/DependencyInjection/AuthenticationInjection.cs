﻿using System.Text;
using Gateway.Core.Configs;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Core.DependencyInjection;

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.SecretKey))
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
