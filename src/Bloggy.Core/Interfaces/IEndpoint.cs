using Microsoft.AspNetCore.Builder;

namespace Bloggy.Core.Interfaces;

public interface IEndpoint
{
    void MapEndpoints(WebApplication app);
}