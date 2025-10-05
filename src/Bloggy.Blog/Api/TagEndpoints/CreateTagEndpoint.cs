using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Tags;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api.TagEndpoints;

public class CreateTagEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for creating a tag
        app.MapGroup(Routes.TagBaseRoute)
            .WithSummary("Create Tag")
            .MapPost("/", async (IOperationService operations,
                [FromHeader] string requestedBy,
                [FromBody] CreateTagRequest request) =>
            {
                // Operation
                var operationResult = await operations.CreateTag.ExecuteAsync(
                    new CreateTagCommand(request.Name, request.Slug));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(
                        new CreateTagResponse(
                            TagId: operationResult.Value!
                        )),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.Failed => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.TagEndpointGroupTag)
            .WithDescription("Creates a new tag.")
            .Produces<CreateTagResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record CreateTagRequest(string Name, string Slug);
public record CreateTagResponse(string TagId);