using System.Text.Json.Serialization;
using Common.Extensions;
using Common.Helpers;
using Identity.Application.Interfaces;
using Identity.Core.Bootstrap;
using Identity.Infrastructure.Database;
using Serilog;

var env = BootstrapHelper.GetEnvironmentName("Local");
var configs = BootstrapHelper.GetConfigFromAppsettingsJson(env);

// Logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configs)
    .Enrich.WithMachineName()
    .CreateLogger();

// Create app builder
var builder = WebApplication.CreateBuilder();

// Use Serilog as logging provider
builder.Logging.ClearProviders();
builder.Host.UseSerilog(Log.Logger);

// Add configs
builder.Configuration.AddConfiguration(configs);


// Configure JSON options to serialize enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add services to the container
builder.Services.AddCustomConfigurations(configs);
builder.Services.AddConfiguredMediatR();

builder.Services.AddConfiguredMongoDB(configs);
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

builder.Services.AddConfiguredRedisCache(configs);

builder.Services.AddConfiguredBrevo(configs);

builder.Services.AddHealthChecks();
builder.Services.AddConfiguredOpenApi();

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
app.MapHealthChecks("/health");

// Add endpoints
app.MapEndpoints();

if (!app.Environment.IsProduction())
    app.UseConfiguredSwagger();

try { await app.RunAsync(); }
catch (Exception ex) { Log.Fatal(ex, "Application failed to start."); }