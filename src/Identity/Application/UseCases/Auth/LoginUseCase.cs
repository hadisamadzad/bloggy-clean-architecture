using Common.Helpers;
using Common.Utilities.OperationResult;
using FluentValidation;
using Identity.Application.Constants;
using Identity.Application.Helpers;
using Identity.Application.Interfaces;
using Identity.Application.Types.Models.Auth;
using MediatR;

namespace Identity.Application.UseCases.Auth;

// Handler
public class LoginHandler(IRepositoryManager repository) :
    IRequestHandler<LoginCommand, OperationResult>
{
    public async Task<OperationResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Validation
        var validation = new LoginValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        // Get
        var user = await repository.Users.GetByEmailAsync(request.Email);
        if (user is null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.InvalidId);

        // Lockout check
        if (user.IsLockedOutOrNotActive())
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.InvalidCredentials);

        // Login check via password
        var loggedIn = PasswordHelper.CheckPasswordHash(user.PasswordHash, request.Password);

        // Lockout history
        if (!loggedIn)
        {
            user.TryToLockout();
            _ = await repository.Users.UpdateAsync(user);
            await repository.CommitAsync();
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.InvalidCredentials);
        }

        /* Here user is authenticated */
        user.LastLoginDate = DateTime.UtcNow;
        user.ResetLockoutHistory();
        _ = await repository.Users.UpdateAsync(user);

        var result = new LoginResult
        {
            Email = user.Email,
            FullName = user.GetFullName(),
            AccessToken = user.CreateJwtAccessToken(),
            RefreshToken = user.CreateJwtRefreshToken()
        };

        return OperationResult.Success(result);
    }
}

// Model
public record LoginCommand(
    string Email,
    string Password) : IRequest<OperationResult>;

// Model Validator
public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithState(_ => Errors.InvalidEmail);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithState(_ => Errors.InvalidPassword);
    }
}