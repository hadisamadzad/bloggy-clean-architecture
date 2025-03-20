using Common.Helpers;
using Common.Utilities.OperationResult;
using FluentValidation;
using Identity.Application.Constants;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.UseCases.Users;

// Handler
public class UpdateUserHandler(IRepositoryManager repository) :
    IRequestHandler<UpdateUserCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // Validation
        var validation = new UpdateUserValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        // Check if user is admin
        var requesterUser = await repository.Users.GetByIdAsync(request.AdminUserId);
        if (requesterUser is null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.InvalidId);

        // Get
        var user = await repository.Users.GetByIdAsync(request.UserId);
        if (user is null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.InvalidId);

        // Update
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        user.UpdatedAt = DateTime.UtcNow;
        _ = await repository.Users.UpdateAsync(user);

        return OperationResult.Success(user.Id);
    }
}

// Model
public record UpdateUserCommand(
    string AdminUserId,
    string UserId,
    string FirstName,
    string LastName
) : IRequest<OperationResult>;

// Model Validator
public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        // User id
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithState(_ => Errors.InvalidId);

        // First name
        RuleFor(x => x.FirstName)
            .Length(2, 80)
            .When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithState(_ => Errors.InvalidFirstName);

        // Last name
        RuleFor(x => x.LastName)
            .Length(2, 80)
            .When(x => !string.IsNullOrEmpty(x.LastName))
            .WithState(_ => Errors.InvalidLastName);
    }
}