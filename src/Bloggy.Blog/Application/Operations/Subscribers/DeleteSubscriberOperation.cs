using Bloggy.Blog.Application.Constants;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using FluentValidation;

namespace Bloggy.Blog.Application.Operations.Subscribers;

public class DeleteSubscriberOperation(IRepositoryManager repository) :
    IOperation<DeleteSubscriberCommand, NoResult>
{
    public async Task<OperationResult<NoResult>> ExecuteAsync(
        DeleteSubscriberCommand command, CancellationToken? cancellation = null)
    {
        // Validate
        var validation = new DeleteSubscriberValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult.ValidationFailure([.. validation.GetErrorMessages()]);

        command = command with { Email = command.Email.ToLower() };

        var entity = await repository.Subscribers.GetByEmailAsync(command.Email);
        if (entity is null)
            return OperationResult.NoOperation();

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Subscribers.UpdateAsync(entity);

        return OperationResult.Success();
    }
}

public record DeleteSubscriberCommand(string Email) : IOperationCommand;

// Validator
public class DeleteSubscriberValidator : AbstractValidator<DeleteSubscriberCommand>
{
    public DeleteSubscriberValidator()
    {
        // Email
        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(100)
            .WithState(_ => Errors.InvalidEmail);
    }
}