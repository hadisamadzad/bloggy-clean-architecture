using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Models.Users;

namespace Bloggy.Identity.Application.Operations.Auth;

public class GetUserProfileOperation(
    IRepositoryManager repository) :
    IOperation<GetUserProfileCommand, UserModel>
{
    public async Task<OperationResult<UserModel>> ExecuteAsync(
        GetUserProfileCommand command, CancellationToken cancellation)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(command.RequestedBy))
            return OperationResult<UserModel>.ValidationFailure(["Invalid userId"]);

        // Get
        var user = await repository.Users.GetByIdAsync(command.RequestedBy);
        if (user is null)
            return OperationResult<UserModel>.Failure("User not found");

        // Mapping
        var response = user.MapToUserModel();

        return OperationResult<UserModel>.Success(response);
    }
}

public record GetUserProfileCommand(string RequestedBy) : IOperationCommand;