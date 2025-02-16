using System.Text.Json.Serialization;
using Blog.Api;
using Blog.Application.Interfaces;
using Blog.Core.Bootstrap;
using Blog.Infrastructure.Database;
using Common.Helpers;
using Common.Persistence.MongoDB;
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
//builder.Logging.ClearProviders();
builder.Host.UseSerilog(Log.Logger);

// Add configs
builder.Configuration.AddConfiguration(configs);


// Configure JSON options to serialize enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configure cors
builder.Services.AddCors(options => options
    .AddPolicy("general", policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));

// Add services to the container
builder.Services.AddCustomConfigurations(configs);
builder.Services.AddConfiguredMediatR();

builder.Services.AddConfiguredMongoDB(configs);
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

//builder.Services.AddConfiguredRedisCache(configs);

builder.Services.AddHealthChecks();
builder.Services.AddConfiguredSwagger();

WebApplication app = default!;
try
{
    app = builder.Build();
    Log.Information($"Application started on: {configs["urls"]} ({env})");
}
catch (Exception ex)
{
    Log.Fatal(ex, $"Application failed to build.");
}
if (app is null) return;

// Add middleware
app.UseCors("general");
app.MapHealthChecks("/health");

// Add endpoints
app.MapEndpoints();

if (!app.Environment.IsProduction())
    app.UseConfiguredSwagger();

//MigrationRunner.Run(app.Services);

try { app.Run(); }
catch (Exception ex) { Log.Fatal(ex, "Application failed to start."); }