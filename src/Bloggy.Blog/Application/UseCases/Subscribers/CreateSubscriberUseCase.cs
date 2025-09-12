using Bloggy.Blog.Application.Constants;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using FluentValidation;
using MediatR;

namespace Bloggy.Blog.Application.UseCases.Subscribers;

// Handler
public class CreateSubscriberHandler(IRepositoryManager repository) :
    IRequestHandler<CreateSubscriberCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CreateSubscriberCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validation = new CreateSubscriberValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

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

        return OperationResult.Success(entity);
    }
}

// Model
public record CreateSubscriberCommand(string Email) : IRequest<OperationResult>;

// Model Validator
public class CreateSubscriberValidator : AbstractValidator<CreateSubscriberCommand>
{
    public CreateSubscriberValidator()
    {
        // Email
        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(100)
            .WithState(_ => Errors.InvalidEmail);
    }
}