﻿using Common.Helpers;
using Common.Utilities.OperationResult;
using FluentValidation;
using Identity.Application.Constants;
using Identity.Application.Helpers;
using Identity.Application.Interfaces;
using Identity.Application.Specifications.Auth;
using MediatR;

namespace Identity.Application.UseCases.Users;

// Handler
public class UpdateUserPasswordHandler(IRepositoryManager repository) :
    IRequestHandler<UpdateUserPasswordCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        // Validation
        var validation = new UpdateUserPasswordValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        // Get
        var user = await repository.Users.GetByIdAsync(request.UserId);
        if (user is null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.InvalidId);

        if (PasswordHelper.CheckPasswordHash(user.PasswordHash, request.CurrentPassword))
            user.PasswordHash = PasswordHelper.Hash(request.NewPassword);
        else
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.InvalidCredentials);

        user.UpdatedAt = DateTime.UtcNow;
        _ = await repository.Users.UpdateAsync(user);

        return OperationResult.Success(user.Id);
    }
}

// Model
public record UpdateUserPasswordCommand(
    string AdminUserId,
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<OperationResult>;

// Model Validator
public class UpdateUserPasswordValidator : AbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithState(_ => Errors.InvalidPassword);

        RuleFor(x => x.NewPassword)
            .Must(x => new AcceptablePasswordStrengthSpecification().IsSatisfiedBy(x))
            .WithState(_ => Errors.WeakPassword);
    }
}