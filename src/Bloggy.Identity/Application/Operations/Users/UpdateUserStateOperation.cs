using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Entities;
using FluentValidation;

namespace Bloggy.Identity.Application.Operations.Users;

public class UpdateUserStateOperation(IRepositoryManager repository) :
    IOperation<UpdateUserStateCommand, NoResult>
{
    public async Task<OperationResult<NoResult>> ExecuteAsync(
        UpdateUserStateCommand command, CancellationToken? cancellation = null)
    {
        // Validation
        var validation = new UpdateUserStateValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult.ValidationFailure([.. validation.GetErrorMessages()]);

        // Get
        var user = await repository.Users.GetByIdAsync(command.UserId);
        if (user is null)
            return OperationResult.Failure("User not found");

        // Update
        user.State = command.State;
        user.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Users.UpdateAsync(user);

        return OperationResult.Success();
    }
}

public record UpdateUserStateCommand(
    string AdminUserId,
    string UserId,
    UserState State) : IOperationCommand;

public class UpdateUserStateValidator : AbstractValidator<UpdateUserStateCommand>
{
    public UpdateUserStateValidator()
    {
        // User id
        RuleFor(x => x.UserId)
            .NotEmpty();

        // User State
        RuleFor(x => x.State)
            .IsInEnum();
    }
}