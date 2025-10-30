using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Tags;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api.TagEndpoints;

public class UpdateTagEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for updating a tag
        app.MapGroup(Routes.TagBaseRoute)
            .WithSummary("Update Tag")
            .MapPut("{tagId}/", async (IOperationService operations,
                [FromRoute] string tagId,
                [FromBody] UpdateTagRequest request) =>
            {
                // Operation
                var operationResult = await operations.UpdateTag.ExecuteAsync(
                    new UpdateTagCommand(tagId, request.Name, request.Slug));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.Failed => Results.UnprocessableEntity(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.TagEndpointGroupTag)
            .WithDescription("Updates an existing tag.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record UpdateTagRequest(string Name, string Slug);
