using Common.Helpers;
using Common.Utilities.OperationResult;
using FluentValidation;
using Identity.Application.Constants;
using Identity.Application.Helpers;
using Identity.Application.Interfaces;
using Identity.Application.Types.Entities;
using MediatR;

namespace Identity.Application.UseCases.Users;

// Handler
public class CreateUserHandler(IRepositoryManager repository) :
    IRequestHandler<CreateUserCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Validation
        var validation = new CreateUserValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        // Check if user is admin
        var requesterUser = await repository.Users.GetByIdAsync(request.AdminUserId);
        if (requesterUser is null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.InvalidId);

        // Check role
        if (!requesterUser.HasAdminRole())
            return OperationResult.Failure(OperationStatus.Unauthorized, Errors.InsufficientAccessLevel);

        // Checking duplicate email
        var isDuplicate = await repository.Users
            .ExistsAsync(x => x.Email.ToLower() == request.Email.ToLower());
        if (isDuplicate)
            return OperationResult.Failure(OperationStatus.Failed, Errors.DuplicateUsername);

        // Factory
        var entity = new UserEntity
        {
            Id = UidHelper.GenerateNewId("user"),
            PasswordHash = PasswordHelper.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            IsEmailConfirmed = true,
            State = UserState.Active,
            Role = Role.User,
            SecurityStamp = UserHelper.CreateUserStamp(),
            ConcurrencyStamp = UserHelper.CreateUserStamp(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.Users.InsertAsync(entity);

        return OperationResult.Success(entity);
    }
}

// Model
public record CreateUserCommand(
    string AdminUserId,
    string Email,
    string Password) : IRequest<OperationResult>
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
}

// Model Validator
public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        // Id
        RuleFor(x => x.AdminUserId)
            .NotEmpty()
            .WithState(_ => Errors.InvalidId);

        // Email
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithState(_ => Errors.InvalidEmail);

        // Password
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithState(_ => Errors.InvalidPassword);

        // First name
        RuleFor(x => x.FirstName)
            .MaximumLength(80)
            .WithState(_ => Errors.InvalidFirstName);

        // Last name
        RuleFor(x => x.LastName)
            .MaximumLength(80)
            .WithState(_ => Errors.InvalidLastName);

    }
}