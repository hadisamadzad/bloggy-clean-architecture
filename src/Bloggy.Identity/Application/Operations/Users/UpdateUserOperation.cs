using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using FluentValidation;

namespace Bloggy.Identity.Application.Operations.Users;

public class UpdateUserOperation(IRepositoryManager repository) :
    IOperation<UpdateUserCommand, NoResult>
{
    public async Task<OperationResult> ExecuteAsync(
        UpdateUserCommand command, CancellationToken? cancellation = null)
    {
        // Validation
        var validation = new UpdateUserValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult.ValidationFailure([.. validation.GetErrorMessages()]);

        // Check if user is admin
        var requesterUser = await repository.Users.GetByIdAsync(command.AdminUserId);
        if (requesterUser is null)
            return OperationResult.AuthorizationFailure("Access denied");

        // Get
        var user = await repository.Users.GetByIdAsync(command.UserId);
        if (user is null)
            return OperationResult.NotFoundFailure("User not found");

        // Update
        user.FirstName = command.FirstName;
        user.LastName = command.LastName;

        user.UpdatedAt = DateTime.UtcNow;
        _ = await repository.Users.UpdateAsync(user);

        return OperationResult.Success();
    }
}

public record UpdateUserCommand(
    string AdminUserId,
    string UserId,
    string FirstName,
    string LastName
) : IOperationCommand;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        // User id
        RuleFor(x => x.UserId)
            .NotEmpty();

        // First name
        RuleFor(x => x.FirstName)
            .Length(2, 80)
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        // Last name
        RuleFor(x => x.LastName)
            .Length(2, 80)
            .When(x => !string.IsNullOrEmpty(x.LastName));
    }
}