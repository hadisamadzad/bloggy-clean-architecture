using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Models.Auth;

namespace Bloggy.Identity.Application.Operations.Auth;

public class GetNewAccessTokenOperation(
    IRepositoryManager repository) :
    IOperation<GetNewAccessTokenCommand, TokenResult>
{
    public async Task<OperationResult<TokenResult>> ExecuteAsync(
        GetNewAccessTokenCommand command, CancellationToken? cancellation = null)
    {
        if (!JwtHelper.IsValidJwtRefreshToken(command.RefreshToken))
            return OperationResult<TokenResult>.ValidationFailure(["Invalid refresh token"]);

        var email = JwtHelper.GetEmail(command.RefreshToken);

        var user = await repository.Users.GetByEmailAsync(email);
        if (user is null)
            return OperationResult<TokenResult>.NotFoundFailure("User not found");

        // Lockout check
        if (user.IsLockedOutOrNotActive())
            return OperationResult<TokenResult>.AuthorizationFailure("User is locked out or not active");

        var result = new TokenResult
        {
            AccessToken = user.CreateJwtAccessToken(),
        };

        return OperationResult<TokenResult>.Success(result);
    }
}

public record GetNewAccessTokenCommand(string RefreshToken) : IOperationCommand;