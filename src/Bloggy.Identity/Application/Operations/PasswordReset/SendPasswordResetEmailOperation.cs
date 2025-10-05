using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Configs;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Bloggy.Identity.Application.Operations.PasswordReset;

public class SendPasswordResetEmailOperation(
    IRepositoryManager repository,
    IEmailService transactionalEmailService,
    IOptions<PasswordResetConfig> passwordResetConfig)
    : IOperation<SendPasswordResetEmailCommand, NoResult>
{
    private readonly PasswordResetConfig _passwordResetConfig = passwordResetConfig.Value;

    public async Task<OperationResult> ExecuteAsync(
        SendPasswordResetEmailCommand command, CancellationToken? cancellation = null)
    {
        // Validation
        var validation = new SendPasswordResetEmailValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult.ValidationFailure([.. validation.GetErrorMessages()]);

        // Get
        var user = await repository.Users.GetByEmailAsync(command.Email);
        if (user is null)
            return OperationResult.NotFoundFailure("User not found");

        if (user.IsLockedOutOrNotActive())
            return OperationResult.AuthorizationFailure("User is locked out or not active");

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

        return OperationResult.Success();
    }
}

public record SendPasswordResetEmailCommand(string Email) : IOperationCommand;

public class SendPasswordResetEmailValidator : AbstractValidator<SendPasswordResetEmailCommand>
{
    public SendPasswordResetEmailValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();
    }
}