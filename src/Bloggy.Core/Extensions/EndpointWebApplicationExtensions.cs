using Bloggy.Core.Interfaces;
using Microsoft.AspNetCore.Builder;

namespace Bloggy.Core.Extensions;

public static class EndpointWebApplicationExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        var interfaceType = typeof(IEndpoint);
        var endpointTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsInterface && !x.IsAbstract &&
                interfaceType.IsAssignableFrom(x))
            .ToList();

        foreach (var endpointType in endpointTypes)
        {
            var instance = Activator.CreateInstance(endpointType);
            interfaceType.GetMethods().Single()
                .Invoke(instance, [app]);
        }
    }
}