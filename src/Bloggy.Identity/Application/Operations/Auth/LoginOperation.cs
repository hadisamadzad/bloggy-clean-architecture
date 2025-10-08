using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Models.Auth;
using FluentValidation;
using Identity.Application.Helpers;

namespace Bloggy.Identity.Application.Operations.Auth;

public class LoginOperation(IRepositoryManager repository) :
    IOperation<LoginCommand, LoginResult>
{
    public async Task<OperationResult<LoginResult>> ExecuteAsync(
        LoginCommand command, CancellationToken? cancellation = null)
    {
        // Validation
        var validation = new LoginValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult<LoginResult>.ValidationFailure([.. validation.GetErrorMessages()]);

        // Get
        var user = await repository.Users.GetByEmailAsync(command.Email);
        if (user is null)
            return OperationResult<LoginResult>.NotFoundFailure("User not found");

        // Lockout check
        if (user.IsLockedOutOrNotActive())
            return OperationResult<LoginResult>.AuthorizationFailure("User is locked out or not active");

        // Login check via password
        var isLoginSuccessful = PasswordHelper.CheckPasswordHash(user.PasswordHash, command.Password);

        // Lockout history
        if (!isLoginSuccessful)
        {
            user.TryToLockout();
            _ = await repository.Users.UpdateAsync(user);
            return OperationResult<LoginResult>.AuthorizationFailure("Invalid credentials");
        }

        /* Here user is authenticated */
        user.LastLoginDate = DateTime.UtcNow;
        user.ResetLockoutHistory();
        _ = await repository.Users.UpdateAsync(user);

        var result = new LoginResult
        (
            Email: user.Email,
            FullName: user.GetFullName(),
            AccessToken: user.CreateJwtAccessToken(),
            RefreshToken: user.CreateJwtRefreshToken(),
            RefreshTokenMaxAge: JwtHelper.RefreshTokenMaxAge
        );

        return OperationResult<LoginResult>.Success(result);
    }
}

public record LoginCommand(
    string Email,
    string Password) : IOperationCommand;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}