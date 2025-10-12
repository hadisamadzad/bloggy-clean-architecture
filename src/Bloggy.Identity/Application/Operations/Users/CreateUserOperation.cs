using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Entities;
using Bloggy.Identity.Application.Types.Models.Users;
using FluentValidation;
using Identity.Application.Helpers;

namespace Bloggy.Identity.Application.Operations.Users;

public class CreateUserOperation(IRepositoryManager repository) :
    IOperation<CreateUserCommand, string>
{
    public async Task<OperationResult<string>> ExecuteAsync(
        CreateUserCommand command, CancellationToken? cancellation = null)
    {
        // Validation
        var validation = new CreateUserValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult<string>.ValidationFailure([.. validation.GetErrorMessages()]);

        // Check if user is admin
        var requesterUser = await repository.Users.GetByIdAsync(command.AdminUserId);
        if (requesterUser is null)
            return OperationResult<string>.NotFoundFailure("User not found");

        // Check role
        if (!requesterUser.HasAdminRole())
            return OperationResult<string>.AuthorizationFailure("Access denied");

        // Checking duplicate email
        var isDuplicate = await repository.Users
            .ExistsAsync(x => x.Email.ToLower() == command.Email.ToLower());
        if (isDuplicate)
            return OperationResult<string>.Failure("Email already exists");

        // Factory
        var entity = new UserEntity
        {
            Id = UidHelper.GenerateNewId("user"),
            PasswordHash = PasswordHelper.Hash(command.Password),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            IsEmailConfirmed = true,
            Status = UserState.Active,
            Role = Role.User,
            SecurityStamp = UserHelper.CreateUserStamp(),
            ConcurrencyStamp = UserHelper.CreateUserStamp(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.Users.InsertAsync(entity);
        var model = entity.MapToUserModel();

        return OperationResult<string>.Success(model.UserId);
    }
}

public record CreateUserCommand(
    string AdminUserId,
    string Email,
    string Password) : IOperationCommand
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        // Id
        RuleFor(x => x.AdminUserId)
            .NotEmpty();

        // Email
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        // Password
        RuleFor(x => x.Password)
            .NotEmpty();

        // First name
        RuleFor(x => x.FirstName)
            .MaximumLength(80);

        // Last name
        RuleFor(x => x.LastName)
            .MaximumLength(80);
    }
}