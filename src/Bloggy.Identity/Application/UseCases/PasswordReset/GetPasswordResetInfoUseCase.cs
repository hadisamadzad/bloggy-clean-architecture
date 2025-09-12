using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Constants;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using MediatR;

namespace Bloggy.Identity.Application.UseCases.PasswordReset;

// Handler
public class GetPasswordResetInfoHandler(IRepositoryManager repository)
    : IRequestHandler<GetPasswordResetInfoQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetPasswordResetInfoQuery request, CancellationToken cancellationToken)
    {
        var (succeeded, email) = PasswordResetTokenHelper.ReadPasswordResetToken(request.Token);

        if (!succeeded)
            return OperationResult.Failure(OperationStatus.Failed, Errors.InvalidToken);

        var user = await repository.Users.GetByEmailAsync(email) ??
            throw new AggregateException($"Unable to read the valid password-reset token: {request.Token}");

        if (user.IsLockedOutOrNotActive())
            return OperationResult.Failure(OperationStatus.Failed, Errors.LockedUser);

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            return OperationResult.Failure(OperationStatus.Failed, Errors.InvalidToken);

        return OperationResult.Success(user.Email);
    }
}

// Model
public record GetPasswordResetInfoQuery(string Token) : IRequest<OperationResult>;