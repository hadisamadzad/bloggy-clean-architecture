using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Entities;
using Bloggy.Identity.Application.Types.Models.Auth;
using FluentValidation;
using Identity.Application.Helpers;

namespace Bloggy.Identity.Application.Operations.Auth;

public class RegisterOperation(IRepositoryManager repository)
    : IOperation<RegisterCommand, RegisterResult>
{
    public async Task<OperationResult<RegisterResult>> ExecuteAsync(
        RegisterCommand command, CancellationToken? cancellation = null)
    {
        // Validation
        var validation = new RegisterValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult<RegisterResult>.ValidationFailure([.. validation.GetErrorMessages()]);

        // Check initial ownership
        // NOTE Registration is supposed to be done only once and for the first user. So if
        // there is any existing user, it means there is nothing to do with registration.
        var isAlreadyOwned = await repository.Users.AnyAsync();
        if (isAlreadyOwned)
            return OperationResult<RegisterResult>.Failure("Registration is already done");

        var user = new UserEntity
        {
            Id = UidHelper.GenerateNewId("user"),
            Email = command.Email.ToLower(),
            PasswordHash = PasswordHelper.Hash(command.Password),
            State = UserState.Active,
            Role = Role.Owner, // The first user will be the owner
            SecurityStamp = UserHelper.CreateUserStamp(),
            ConcurrencyStamp = UserHelper.CreateUserStamp(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.Users.InsertAsync(user);

        var result = new RegisterResult
        {
            UserId = user.Id,
            Email = user.Email
        };

        return OperationResult<RegisterResult>.Success(result);
    }
}

public record RegisterCommand(string Email, string Password) : IOperationCommand;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .Must(password => PasswordHelper.CheckStrength(password) >= PasswordScore.Medium)
            .WithMessage("Password is not strong enough");
    }
}