using System.Text.Json.Serialization;
using Bloggy.Core.Helpers;
using Bloggy.Gateway.Core;
using Bloggy.Gateway.Core.DependencyInjection;
using Bloggy.Gateway.Core.Middleware;
using Ocelot.DependencyInjection;
using Serilog;

var env = BootstrapHelper.GetEnvironmentName("Local");
var configs = BootstrapHelper.GetConfigFromAppsettingsJson(env);

// Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configs)
    .Enrich.WithMachineName()
    .CreateLogger();

var builder = WebApplication.CreateBuilder();

// Use Serilog as logging provider
builder.Logging.ClearProviders();
builder.Host.UseSerilog(Log.Logger);

builder.Configuration.AddConfiguration(configs);
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

WebApplication app = default!;
try
{
    app = builder.Build();
    Log.Information("Application started on: {0} ({1})", configs["Urls"], env);
}
catch (Exception ex)
{
    Log.Fatal(ex, $"Application failed to build.");
}
if (app is null) return;

// Add middleware

if (builder.Environment.IsProduction())
    app.UseHsts();

app.UseCors(Constants.CorsPolicyName);
app.UseHealthChecks("/health");

app.UseConfiguredOcelot();

try { await app.RunAsync(); }
catch (Exception ex) { Log.Fatal(ex, "Application failed to start."); }