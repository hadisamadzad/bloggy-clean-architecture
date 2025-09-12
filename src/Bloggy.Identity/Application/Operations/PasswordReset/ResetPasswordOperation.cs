using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Configs;
using FluentValidation;
using Identity.Application.Helpers;
using Microsoft.Extensions.Options;

namespace Bloggy.Identity.Application.Operations.PasswordReset;

// Handler
public class ResetPasswordOperation(IRepositoryManager repository,
    IOptions<PasswordResetConfig> passwordResetConfig)
    : IOperation<ResetPasswordCommand, string>
{
    private readonly PasswordResetConfig _passwordResetConfig = passwordResetConfig.Value;

    public async Task<OperationResult<string>> ExecuteAsync(
        ResetPasswordCommand command, CancellationToken cancellation)
    {
        // Validation
        var validation = new ResetPasswordValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult<string>.ValidationFailure([.. validation.GetErrorMessages()]);

        // Get
        var (succeeded, email) = PasswordResetTokenHelper.ReadPasswordResetToken(command.Token);
        if (!succeeded)
            return OperationResult<string>.Failure("Invalid token");

        var user = await repository.Users.GetByEmailAsync(email);
        if (user is null)
            return OperationResult<string>.Failure("User not found");

        if (user.IsLockedOutOrNotActive())
            return OperationResult<string>.Failure("User is locked out or not active");

        user.PasswordHash = PasswordHelper.Hash(command.NewPassword);

        _ = await repository.Users.UpdateAsync(user);

        return OperationResult<string>.Success(user.Id);
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