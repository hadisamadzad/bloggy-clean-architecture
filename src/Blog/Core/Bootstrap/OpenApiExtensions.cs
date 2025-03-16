namespace Blog.Core.Bootstrap;

public static class OpenApiExtensions
{
    private const string DocumentTitle = "BloggyAPI";

    public static IServiceCollection AddConfiguredOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(DocumentTitle);

        return services;
    }

    public static void UseConfiguredSwagger(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwaggerUI(configs =>
        {
            configs.DocumentTitle = "Swagger UI - Bloggy Blog API";
            configs.SwaggerEndpoint($"/openapi/{DocumentTitle}.json", "Bloggy API - Blog");
        });
    }
}