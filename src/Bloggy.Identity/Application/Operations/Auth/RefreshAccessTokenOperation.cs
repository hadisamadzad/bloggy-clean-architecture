using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;

namespace Bloggy.Identity.Application.Operations.Auth;

public class RefreshAccessTokenOperation(
    IRepositoryManager repository) :
    IOperation<RefreshAccessTokenCommand, string>
{
    public async Task<OperationResult<string>> ExecuteAsync(
        RefreshAccessTokenCommand command, CancellationToken? cancellation = null)
    {
        if (!JwtHelper.IsValidJwtRefreshToken(command.RefreshToken))
            return OperationResult<string>.ValidationFailure(["Invalid refresh token"]);

        var email = JwtHelper.GetEmail(command.RefreshToken);

        var user = await repository.Users.GetByEmailAsync(email);
        if (user is null)
            return OperationResult<string>.NotFoundFailure("User not found");

        // Lockout check
        if (user.IsLockedOutOrNotActive())
            return OperationResult<string>.AuthorizationFailure("User is locked out or not active");

        var accessToken = user.CreateJwtAccessToken();

        return OperationResult<string>.Success(accessToken);
    }
}

public record RefreshAccessTokenCommand(string RefreshToken) : IOperationCommand;