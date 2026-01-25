using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
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

// Rate limiting (gateway-level)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Views endpoint: partition by VisitorId header if present, otherwise by IP
        if (path.StartsWith("/api/blog/views/article", StringComparison.OrdinalIgnoreCase))
            return RateLimitPartition.GetFixedWindowLimiter(context.Request.Headers["VisitorId"].FirstOrDefault() ?? context.Connection.RemoteIpAddress?.ToString() ?? "anon", _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 60, // 60 requests
                    Window = TimeSpan.FromMinutes(1),
                    AutoReplenishment = true,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });

        // Login: strict per-IP
        if (path.Contains("/api/identity/auth/login", StringComparison.OrdinalIgnoreCase) || path.Contains("/api/identity/auth/register", StringComparison.OrdinalIgnoreCase))
            return RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "anon", _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(10),
                    AutoReplenishment = true,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });

        // Password reset send: per-email (header) or per-IP
        if (path.Contains("/api/identity/auth/password-reset", StringComparison.OrdinalIgnoreCase))
            return RateLimitPartition.GetFixedWindowLimiter(context.Request.Headers["Email"].FirstOrDefault() ?? context.Connection.RemoteIpAddress?.ToString() ?? "anon", _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 1,
                    Window = TimeSpan.FromHours(1),
                    AutoReplenishment = true,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });

        // Subscriber creation: gentle per-IP
        if (path.Contains("/api/blog/subscribers", StringComparison.OrdinalIgnoreCase))
            return RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "anon", _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(60),
                    AutoReplenishment = true,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });

        // Writes (catch-all under /api/blog for non-GET): per-IP
        if (path.StartsWith("/api/blog/", StringComparison.OrdinalIgnoreCase) && !string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
            return RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "anon", _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100, // write ops per 10 minutes per IP
                    Window = TimeSpan.FromMinutes(10),
                    AutoReplenishment = true,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });

        // Default: per-IP
        return RateLimitPartition.GetFixedWindowLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "anon", _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 1000,
                Window = TimeSpan.FromMinutes(1),
                AutoReplenishment = true,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
});

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
app.UseHealthChecks("/api/health");

// Apply rate limiting middleware before the gateway proxy
app.UseRateLimiter();

app.UseConfiguredOcelot();

try { await app.RunAsync(); }
catch (Exception ex) { Log.Fatal(ex, "Application failed to start."); }