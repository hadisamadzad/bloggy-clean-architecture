using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Configs;
using FluentValidation;
using Identity.Application.Helpers;
using Microsoft.Extensions.Options;

namespace Bloggy.Identity.Application.Operations.PasswordReset;

public class ResetPasswordOperation(IRepositoryManager repository,
    IOptions<PasswordResetConfig> passwordResetConfig)
    : IOperation<ResetPasswordCommand, NoResult>
{
    private readonly PasswordResetConfig _passwordResetConfig = passwordResetConfig.Value;

    public async Task<OperationResult<NoResult>> ExecuteAsync(
        ResetPasswordCommand command, CancellationToken? cancellation = default)
    {
        // Validation
        var validation = new ResetPasswordValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult.ValidationFailure([.. validation.GetErrorMessages()]);

        // Get
        var (succeeded, email) = PasswordResetTokenHelper.ReadPasswordResetToken(command.Token);
        if (!succeeded)
            return OperationResult.Failure("Invalid token");

        var user = await repository.Users.GetByEmailAsync(email);
        if (user is null)
            return OperationResult.Failure("User not found");

        if (user.IsLockedOutOrNotActive())
            return OperationResult.Failure("User is locked out or not active");

        user.PasswordHash = PasswordHelper.Hash(command.NewPassword);

        _ = await repository.Users.UpdateAsync(user);

        return OperationResult.Success();
    }
}

public record ResetPasswordCommand(string Token, string NewPassword) : IOperationCommand;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .Must(x => PasswordHelper.CheckStrength(x) >= PasswordScore.Medium)
            .WithMessage("Password is not strong enough");
    }
}