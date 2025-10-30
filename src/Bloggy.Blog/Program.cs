using System.Text.Json.Serialization;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations;
using Bloggy.Blog.Core.Bootstrap;
using Bloggy.Blog.Infrastructure.Background;
using Bloggy.Blog.Infrastructure.Database;
using Bloggy.Blog.Infrastructure.Redis;
using Bloggy.Core.Extensions;
using Bloggy.Core.Helpers;
using Bloggy.Core.Persistence.MongoDB;
using Bloggy.Core.Utilities.OperationResult;
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
builder.Services.AddOperations();
builder.Services.AddTransient<IOperationService, OperationService>();

// Database
builder.Services.AddConfiguredMongoDB(configs);
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

builder.Services.AddConfiguredRedisCache(configs);
builder.Services.AddSingleton<IViewMemoryRepository, ViewRedisRepository>();

// Hosted services
builder.Services.AddHostedService<ViewsFlushHostedService>();

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
app.MapHealthChecks("/api/health");

// Add endpoints
app.MapEndpoints();

if (!app.Environment.IsProduction())
    app.UseConfiguredSwagger();

try { await app.RunAsync(); }
catch (Exception ex) { Log.Fatal(ex, "Application failed to start."); }