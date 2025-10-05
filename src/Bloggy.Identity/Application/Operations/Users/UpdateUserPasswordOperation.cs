using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using FluentValidation;
using Identity.Application.Helpers;

namespace Bloggy.Identity.Application.Operations.Users;

public class UpdateUserPasswordOperation(IRepositoryManager repository) :
    IOperation<UpdateUserPasswordCommand, NoResult>
{
    public async Task<OperationResult<NoResult>> ExecuteAsync(
        UpdateUserPasswordCommand command, CancellationToken? cancellation = null)
    {
        // Validation
        var validation = new UpdateUserPasswordValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult.ValidationFailure([.. validation.GetErrorMessages()]);

        // Get
        var user = await repository.Users.GetByIdAsync(command.UserId);
        if (user is null)
            return OperationResult.NotFoundFailure("User not found");

        // Check if user password is correct and update
        if (PasswordHelper.CheckPasswordHash(user.PasswordHash, command.CurrentPassword))
            user.PasswordHash = PasswordHelper.Hash(command.NewPassword);
        else
            return OperationResult.Failure("Incorrect current password");

        user.UpdatedAt = DateTime.UtcNow;
        _ = await repository.Users.UpdateAsync(user);

        return OperationResult.Success();
    }
}

public record UpdateUserPasswordCommand(
    string AdminUserId,
    string UserId,
    string CurrentPassword,
    string NewPassword) : IOperationCommand;

public class UpdateUserPasswordValidator : AbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .Must(x => PasswordHelper.CheckStrength(x) >= PasswordScore.Medium)
            .WithMessage("Password is not strong enough");
    }
}