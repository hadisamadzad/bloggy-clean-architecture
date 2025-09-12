using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Types.Entities;
using Bloggy.Identity.Application.Types.Models.Users;
using Bloggy.Identity.Application.UseCases.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Identity.Api;

public class UserEndpoints : IEndpoint
{
    const string Route = "api/users/";
    const string Tag = "Users";

    // Models
    public record CreateUserRequest(
        string Email,
        string Password,
        string FirstName,
        string LastName);
    public record UpdateUserRequest(string FirstName, string LastName);
    public record UpdateUserStateRequest(UserState State);
    public record UpdateUserPasswordRequest(string CurrentPassword, string NewPassword);

    // Endpoints
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup(Route).WithTags(Tag);

        // Endpoint for creating a user
        group.MapPost("", async (
            IMediator mediator,
            [FromHeader] string requestedBy,
            [FromBody] CreateUserRequest request) =>
            {
                return await mediator.Send(new CreateUserCommand(
                    AdminUserId: requestedBy,
                    Email: request.Email,
                    Password: request.Password)
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName
                });
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (UserEntity)operation.Value;
                return new
                {
                    UserId = value.Id,
                    Email = value.Email
                };
            });

        // Endpoint for getting a user
        group.MapGet("{userId}", async (
            IMediator mediator,
            [FromHeader] string requestedBy,
            [FromRoute] string userId) =>
            {
                return await mediator.Send(new GetUserByIdQuery(UserId: userId));
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (UserModel)operation.Value;
                return new
                {
                    UserId = value.UserId,
                    Email = value.Email,
                    Mobile = value.Mobile,
                    Role = value.Role,
                    FirstName = value.FirstName,
                    LastName = value.LastName,
                    FullName = value.FullName,
                    CreatedAt = value.CreatedAt,
                    UpdatedAt = value.UpdatedAt
                };
            });

        // Endpoint for updating a user
        group.MapPatch("{userId}", async (
            IMediator mediator,
            [FromHeader] string requestedBy,
            [FromRoute] string userId,
            [FromBody] UpdateUserRequest request) =>
            {
                return await mediator.Send(new UpdateUserCommand
                (
                    AdminUserId: requestedBy,
                    UserId: userId,
                    FirstName: request.FirstName,
                    LastName: request.LastName
                ));
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (string)operation.Value;
                return new
                {
                    UserId = value
                };
            });

        // Endpoint for updating user state
        group.MapPatch("{userId}/state", async (
            IMediator mediator,
            [FromHeader] string requestedBy,
            [FromRoute] string userId,
            [FromBody] UpdateUserStateRequest request) =>
            {
                return await mediator.Send(new UpdateUserStateCommand
                (
                    AdminUserId: requestedBy,
                    UserId: userId,
                    State: request.State
                ));
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (string)operation.Value;
                return new
                {
                    UserId = value
                };
            });

        // Endpoint for updating user password
        group.MapPatch("{userId}/password", async (
            IMediator mediator,
            [FromHeader] string requestedBy,
            [FromRoute] string userId,
            [FromBody] UpdateUserPasswordRequest request) =>
            {
                return await mediator.Send(new UpdateUserPasswordCommand
                (
                    AdminUserId: requestedBy,
                    UserId: userId,
                    CurrentPassword: request.CurrentPassword,
                    NewPassword: request.NewPassword
                ));
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (string)operation.Value;
                return new
                {
                    UserId = value
                };
            });
    }
}