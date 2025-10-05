using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Subscribers;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api.SubscriberEndpoints;

public class UnsubscribeSubscriberEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for unsubscribing a subscriber
        app.MapGroup(Routes.SubscriberBaseRoute)
            .WithSummary("Unsubscribe Subscriber")
            .MapDelete("/", async (IOperationService operations,
                [FromBody] UnsubscribeSubscriberRequest request) =>
            {
                // Operation
                var operationResult = await operations.DeleteSubscriber.ExecuteAsync(
                    new DeleteSubscriberCommand(request.Email));

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.NoOperation => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.SubscriberEndpointGroupTag)
            .WithDescription("Unsubscribes a subscriber by email.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record UnsubscribeSubscriberRequest(string Email);