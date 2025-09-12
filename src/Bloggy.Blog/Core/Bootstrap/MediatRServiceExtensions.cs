using Bloggy.Blog.Application.UseCases.Articles;

namespace Blog.Core.Bootstrap;

public static class MediatRServiceExtensions
{
    public static IServiceCollection AddConfiguredMediatR(this IServiceCollection services)
    {
        // Handlers
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<CreateArticleCommand>());

        return services;
    }
}
