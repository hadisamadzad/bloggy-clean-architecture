using System.Text.Json.Serialization;
using Common.Helpers;
using Gateway.Core;
using Gateway.Core.DependencyInjection;
using Gateway.Core.Middleware;
using Ocelot.DependencyInjection;
using Serilog;

var env = BootstrapHelper.GetEnvironmentName("Local");
var configs = BootstrapHelper.GetConfigFromAppsettingsJson(env);

// Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configs)
    .Enrich.WithMachineName()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = "Gateway",
    EnvironmentName = env
});

builder.Configuration.AddConfiguration(configs);
builder.Logging.ClearProviders();
builder.Host.UseSerilog(Log.Logger);
builder.Configuration.AddOcelot(Constants.RouteConfigPath, builder.Environment);

// Add services to the container
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddConfiguredCors(configs);
builder.Services.AddConfiguredAuthentication(configs);
builder.Services.AddConfiguredOcelot();

builder.Services.AddConfiguredHealthChecks();

WebApplication app = null;
try { app = builder.Build(); }
catch (Exception ex) { Log.Fatal(ex, "Application failed to build."); }
finally { Log.CloseAndFlush(); }

// Add middleware

//if (builder.Environment.IsProduction())
//    app.UseHsts();

app.UseCors(Constants.CorsPolicyName);
app.UseHealthChecks("/health");

app.UseConfiguredOcelot();

try { app.Run(); }
catch (Exception ex) { Log.Fatal(ex, "Application failed to start."); }
finally { Log.CloseAndFlush(); }