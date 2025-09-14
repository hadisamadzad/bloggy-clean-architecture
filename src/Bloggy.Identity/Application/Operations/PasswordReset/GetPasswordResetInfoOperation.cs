using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;

namespace Bloggy.Identity.Application.Operations.PasswordReset;

public class GetPasswordResetInfoOperation(IRepositoryManager repository)
    : IOperation<GetPasswordResetInfoCommand, string>
{
    public async Task<OperationResult<string>> ExecuteAsync(
        GetPasswordResetInfoCommand command, CancellationToken? cancellation = null)
    {
        var (succeeded, email) = PasswordResetTokenHelper.ReadPasswordResetToken(command.Token);

        if (!succeeded)
            return OperationResult<string>.Failure("Invalid token");

        var user = await repository.Users.GetByEmailAsync(email) ??
            throw new AggregateException($"Unable to read the valid password-reset token: {command.Token}");

        if (user.IsLockedOutOrNotActive())
            return OperationResult<string>.Failure("User is locked out or not active");

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            return OperationResult<string>.Failure("Invalid token");

        return OperationResult<string>.Success(user.Email);
    }
}

public record GetPasswordResetInfoCommand(string Token) : IOperationCommand;