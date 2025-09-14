using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;

namespace Bloggy.Identity.Application.Operations.PasswordReset;

public class GetPasswordResetEmailOperation(IRepositoryManager repository)
    : IOperation<GetPasswordResetEmailCommand, string>
{
    public async Task<OperationResult<string>> ExecuteAsync(
        GetPasswordResetEmailCommand command, CancellationToken? cancellation = null)
    {
        var (succeeded, email) = PasswordResetTokenHelper.ReadPasswordResetToken(command.Token);
        if (!succeeded)
            return OperationResult<string>.ValidationFailure(["Invalid token"]);

        var user = await repository.Users.GetByEmailAsync(email) ??
            throw new AggregateException($"Unable to read the valid password-reset token: {command.Token}");

        if (user.IsLockedOutOrNotActive())
            return OperationResult<string>.AuthorizationFailure("User is locked out or not active");

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            return OperationResult<string>.AuthorizationFailure("Invalid token");

        return OperationResult<string>.Success(user.Email);
    }
}

public record GetPasswordResetEmailCommand(string Token) : IOperationCommand;