using Microsoft.Extensions.Configuration;

namespace Common.Helpers;

public sealed class BootstrapHelper
{
    public static string GetEnvironmentName(string @default) =>
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? @default;

    public static IConfigurationRoot GetConfigFromAppsettingsJson(string env) =>
        new ConfigurationBuilder()
#if RELEASE
            .AddJsonFile("appsettings.json", optional: false)
#elif DEBUG
            .AddJsonFile($"appsettings.{env}.json", optional: false)
#endif
            .AddEnvironmentVariables()
            .Build();
}