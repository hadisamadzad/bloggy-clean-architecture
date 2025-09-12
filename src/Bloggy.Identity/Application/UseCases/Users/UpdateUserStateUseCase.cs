using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Constants;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Entities;
using FluentValidation;
using MediatR;

namespace Bloggy.Identity.Application.UseCases.Users;

// Handler
public class UpdateUserStateHandler(IRepositoryManager repository) :
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
            return OperationResult.Failure(OperationStatus.Failed, Errors.InvalidId);

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
