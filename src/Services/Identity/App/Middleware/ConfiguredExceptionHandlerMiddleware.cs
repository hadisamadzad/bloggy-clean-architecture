using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace Identity.App.Middleware;

internal static class ConfiguredExceptionHandlerMiddleware
{
    public static void UseConfiguredExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseExceptionHandler(errorApp =>
            errorApp.Run(async context =>
            {
                const HttpStatusCode responseStatus = HttpStatusCode.InternalServerError;
                context.Response.StatusCode = (int)responseStatus;
                context.Response.ContentType = "application/json";

                var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                if (error == null)
                    return;

                string json;
                if (env.IsProduction())
                    json = JsonSerializer.Serialize(new
                    {
                        context.Response.StatusCode,
                        Title = responseStatus.ToString(),
                    });
                else
                    json = JsonSerializer.Serialize(new
                    {
                        context.Response.StatusCode,
                        Title = responseStatus.ToString(),
                        error.Message,
                        error.StackTrace
                    });

                await context.Response.WriteAsync(json);
            })
        );
    }
}
