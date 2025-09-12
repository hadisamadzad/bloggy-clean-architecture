using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Constants;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Specifications.Auth;
using Bloggy.Identity.Application.Types.Configs;
using FluentValidation;
using Identity.Application.Helpers;
using MediatR;
using Microsoft.Extensions.Options;

namespace Bloggy.Identity.Application.UseCases.PasswordReset;

// Handler
public class ResetPasswordHandler(IRepositoryManager repository,
    IOptions<PasswordResetConfig> passwordResetConfig)
    : IRequestHandler<ResetPasswordCommand, OperationResult>
{
    private readonly IRepositoryManager _unitOfWork = repository;
    private readonly PasswordResetConfig _passwordResetConfig = passwordResetConfig.Value;

    public async Task<OperationResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // Validation
        var validation = new ResetPasswordValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        // Get
        var (succeeded, email) = PasswordResetTokenHelper.ReadPasswordResetToken(request.Token);
        if (!succeeded)
            return OperationResult.Failure(OperationStatus.Failed, Errors.InvalidToken);

        var user = await _unitOfWork.Users.GetByEmailAsync(email);
        if (user is null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.InvalidId);

        if (user.IsLockedOutOrNotActive())
            return OperationResult.Failure(OperationStatus.Failed, Errors.LockedUser);

        user.PasswordHash = PasswordHelper.Hash(request.NewPassword);

        _ = await _unitOfWork.Users.UpdateAsync(user);

        return OperationResult.Success(user.Id);
    }
}

// Model
public record ResetPasswordCommand(string Token, string NewPassword)
    : IRequest<OperationResult>;

// Model Validator
public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithState(_ => Errors.InvalidToken);

        RuleFor(x => x.NewPassword)
            .Must(x => new AcceptablePasswordStrengthSpecification().IsSatisfiedBy(x))
            .WithState(_ => Errors.WeakPassword);
    }
}