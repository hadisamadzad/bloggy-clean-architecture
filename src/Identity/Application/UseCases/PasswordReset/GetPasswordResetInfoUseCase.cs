using Common.Utilities.OperationResult;
using Identity.Application.Constants;
using Identity.Application.Helpers;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.UseCases.PasswordReset;

// Handler
public class GetPasswordResetInfoHandler(IRepositoryManager repository)
    : IRequestHandler<GetPasswordResetInfoQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetPasswordResetInfoQuery request, CancellationToken cancellationToken)
    {
        var (succeeded, email) = PasswordResetTokenHelper.ReadPasswordResetToken(request.Token);

        if (!succeeded)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.InvalidToken);

        var user = await repository.Users.GetByEmailAsync(email) ??
            throw new AggregateException($"Unable to read the valid password-reset token: {request.Token}");

        if (user.IsLockedOutOrNotActive())
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.LockedUser);

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.InvalidToken);

        return OperationResult.Success(user.Email);
    }
}

// Model
public record GetPasswordResetInfoQuery(string Token) : IRequest<OperationResult>;