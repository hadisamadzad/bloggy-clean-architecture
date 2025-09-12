using Microsoft.Extensions.DependencyInjection;

namespace Bloggy.Core.Utilities.OperationResult;

public static class OperationServiceExtensions
{
    public static IServiceCollection AddOperations(this IServiceCollection services)
    {
        var operationTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type => type.GetInterfaces().Where(@interface =>
                @interface.IsGenericType &&
                @interface.GetGenericTypeDefinition() == typeof(IOperation<,>))
            .Select(@interface => new { Service = @interface, Implementation = type }));

        foreach (var type in operationTypes)
        {
            services.AddTransient(type.Service, type.Implementation);
        }

        return services;
    }
}