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
    : IOperation<SendPasswordResetEmailCommand, string>
{
    private readonly PasswordResetConfig _passwordResetConfig = passwordResetConfig.Value;

    public async Task<OperationResult<string>> ExecuteAsync(
        SendPasswordResetEmailCommand request, CancellationToken cancellation)
    {
        // Validation
        var validation = new SendPasswordResetEmailValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult<string>.ValidationFailure([.. validation.GetErrorMessages()]);

        // Get
        var user = await repository.Users.GetByEmailAsync(request.Email);
        if (user is null)
            return OperationResult<string>.Failure("User not found");

        if (user.IsLockedOutOrNotActive())
            return OperationResult<string>.Failure("User is locked out or not active");

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

        return OperationResult<string>.Success(user.Id);
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