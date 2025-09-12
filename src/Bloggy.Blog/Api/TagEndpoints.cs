using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.UseCases.Tags;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api;

public class TagEndpoints : IEndpoint
{
    const string Route = "api/tags/";
    const string Tag = "Tags";

    // Models
    public record CreateTagRequest(string Name, string Slug);

    // Endpoints
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup(Route).WithTags(Tag);

        // Endpoint for creating a tag
        group.MapPost("", async (
            IMediator mediator,
            [FromHeader] string requestedBy,
            [FromBody] CreateTagRequest request) =>
            {
                return await mediator.Send(new CreateTagCommand
                (
                    Name: request.Name,
                    Slug: request.Slug
                ));
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (TagEntity)operation.Value;
                return new
                {
                    TagId = value.Id,
                };
            });
    }
}