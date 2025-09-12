using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Constants;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Configs;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Bloggy.Identity.Application.UseCases.PasswordReset;

// Handler
public class SendPasswordResetEmailHandler(
    IRepositoryManager repository,
    IEmailService transactionalEmailService,
    IOptions<PasswordResetConfig> passwordResetConfig)
    : IRequestHandler<SendPasswordResetEmailCommand, OperationResult>
{
    private readonly PasswordResetConfig _passwordResetConfig = passwordResetConfig.Value;

    public async Task<OperationResult> Handle(SendPasswordResetEmailCommand request, CancellationToken cancellationToken)
    {
        // Validation
        var validation = new SendPasswordResetEmailValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        // Get
        var user = await repository.Users.GetByEmailAsync(request.Email);
        if (user is null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.InvalidId);

        if (user.IsLockedOutOrNotActive())
            return OperationResult.Failure(OperationStatus.Failed, Errors.LockedUser);

        var expirationTime = ExpirationTimeHelper
            .GetExpirationTime(_passwordResetConfig.LinkLifetimeInDays);

        var token = PasswordResetTokenHelper
            .GeneratePasswordResetToken(user.Email, expirationTime);

        var email = user.Email;
        var passwordResetLink = string.Format(_passwordResetConfig.LinkFormat, token);

        var @params = new Dictionary<string, string>
            {
                { "Link", passwordResetLink }
            };

        _ = await transactionalEmailService.SendEmailByTemplateIdAsync(
            _passwordResetConfig.BrevoTemplateId, [email], @params);

        return OperationResult.Success(user.Id);
    }
}

// Model
public record SendPasswordResetEmailCommand(string Email) : IRequest<OperationResult>;

// Model Validator
public class SendPasswordResetEmailValidator : AbstractValidator<SendPasswordResetEmailCommand>
{
    public SendPasswordResetEmailValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithState(_ => Errors.InvalidEmail);
    }
}