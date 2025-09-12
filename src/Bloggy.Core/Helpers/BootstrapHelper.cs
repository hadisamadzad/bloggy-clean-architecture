using Microsoft.Extensions.Configuration;

namespace Bloggy.Core.Helpers;

public static class BootstrapHelper
{
    public static string GetEnvironmentName(string @default) =>
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? @default;

    public static IConfigurationRoot GetConfigFromAppsettingsJson(string env) =>
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
}