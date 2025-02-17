﻿using Blog.Application.Constants;
using Blog.Application.Interfaces;
using Common.Helpers;
using Common.Utilities.OperationResult;
using FluentValidation;
using MediatR;

namespace Blog.Application.UseCases.Subscribers;

// Handler
internal class UnsubscribeSubscriberHandler(IRepositoryManager repository) :
    IRequestHandler<UnsubscribeSubscriberCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UnsubscribeSubscriberCommand request, CancellationToken cancel)
    {
        // Validate
        var validation = new UnsubscribeSubscriberValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        var entity = await repository.Subscribers.GetByEmailAsync(request.Email.ToLower());
        if (entity is null)
            return OperationResult.Success(OperationStatus.Ignored);

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Subscribers.UpdateAsync(entity);

        return OperationResult.Success(entity);
    }
}

// Model
public record UnsubscribeSubscriberCommand(string Email) : IRequest<OperationResult>;

// Model Validator
public class UnsubscribeSubscriberValidator : AbstractValidator<UnsubscribeSubscriberCommand>
{
    public UnsubscribeSubscriberValidator()
    {
        // Email
        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(100)
            .WithState(_ => Errors.InvalidEmail);
    }
}