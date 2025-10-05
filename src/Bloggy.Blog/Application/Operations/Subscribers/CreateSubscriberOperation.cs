using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using FluentValidation;

namespace Bloggy.Blog.Application.Operations.Subscribers;

public class CreateSubscriberOperation(IRepositoryManager repository) :
    IOperation<CreateSubscriberCommand, string>
{
    public async Task<OperationResult<string>> ExecuteAsync(
        CreateSubscriberCommand request, CancellationToken? cancellation = null)
    {
        // Validate
        var validation = new CreateSubscriberValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult<string>.ValidationFailure([.. validation.GetErrorMessages()]);

        request = request with { Email = request.Email.ToLower() };

        var entity = await repository.Subscribers.GetByEmailAsync(request.Email);
        var isNewSubscriber = entity is null;

        entity ??= new SubscriberEntity
        {
            Id = UidHelper.GenerateNewId("subscriber"),
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
        };
        entity.IsActive = true;
        entity.UpdatedAt = DateTime.UtcNow;

        if (isNewSubscriber)
            await repository.Subscribers.InsertAsync(entity);
        else
            _ = await repository.Subscribers.UpdateAsync(entity);

        return OperationResult<string>.Success(entity.Id);
    }
}

public record CreateSubscriberCommand(string Email) : IOperationCommand;

// Validator
public class CreateSubscriberValidator : AbstractValidator<CreateSubscriberCommand>
{
    public CreateSubscriberValidator()
    {
        // Email
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);
    }
}