using Common.Helpers;
using Common.Utilities.OperationResult;
using FluentValidation;
using Identity.Application.Constants;
using Identity.Application.Interfaces;
using Identity.Application.Types.Entities;
using MediatR;

namespace Identity.Application.UseCases.Users;

// Handler
internal class UpdateUserStateHandler(IRepositoryManager repository) :
    IRequestHandler<UpdateUserStateCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateUserStateCommand request, CancellationToken cancellationToken)
    {
        // Validation
        var validation = new UpdateUserStateValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        // Get
        var user = await repository.Users.GetByIdAsync(request.UserId);
        if (user is null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.InvalidId);

        // Update
        user.State = request.State;
        user.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Users.UpdateAsync(user);

        return OperationResult.Success(user.Id);
    }
}

// Model
public record UpdateUserStateCommand(
    string AdminUserId,
    string UserId,
    UserState State) : IRequest<OperationResult>;

// Model Validator
public class UpdateUserStateValidator : AbstractValidator<UpdateUserStateCommand>
{
    public UpdateUserStateValidator()
    {
        // User id
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithState(_ => Errors.InvalidId);

        // Email
        RuleFor(x => x.State)
            .IsInEnum()
            .WithState(_ => Errors.InvalidEmail);
    }
}
