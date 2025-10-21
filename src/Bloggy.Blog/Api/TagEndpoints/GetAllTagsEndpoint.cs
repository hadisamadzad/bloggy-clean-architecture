using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Tags;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;

namespace Bloggy.Blog.Api.TagEndpoints;

public class GetAllTagsEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for getting all tags
        app.MapGroup(Routes.TagBaseRoute)
            .WithSummary("Get All Tags")
            .MapGet("/", async (IOperationService operations) =>
            {
                // Operation
                var operationResult = await operations.GetAllTags.ExecuteAsync(new GetAllTagsCommand());

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new GetAllTagsResponse(
                            Tags: [.. operationResult.Value!.Select(t => new TagResponse(
                                TagId: t.Id,
                                Name: t.Name,
                                Slug: t.Slug
                            ))]
                        )),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.TagEndpointGroupTag)
            .WithDescription("Gets all tags.")
            .Produces<GetAllTagsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record GetAllTagsResponse(List<TagResponse> Tags);
public record TagResponse(string TagId, string Name, string Slug);
