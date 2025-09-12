using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Constants;
using Bloggy.Identity.Application.Helpers;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Models.Auth;
using MediatR;

namespace Bloggy.Identity.Application.UseCases.Auth;

// Handler
public class RefreshTokenHandler(IRepositoryManager repository) : IRequestHandler<RefreshTokenQuery, OperationResult>
{
    public async Task<OperationResult> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        if (!JwtHelper.IsValidJwtRefreshToken(request.RefreshToken))
            return OperationResult.Failure(OperationStatus.Unauthorized, Errors.InvalidCredentials);

        var email = JwtHelper.GetEmail(request.RefreshToken);

        var user = await repository.Users.GetByEmailAsync(email);
        if (user is null)
            return OperationResult.Failure(OperationStatus.Unauthorized, Errors.InvalidId);

        // Lockout check
        if (user.IsLockedOutOrNotActive())
            return OperationResult.Failure(OperationStatus.Unauthorized, Errors.LockedUser);

        var result = new TokenResult
        {
            AccessToken = user.CreateJwtAccessToken(),
        };

        return OperationResult.Success(result);
    }
}

// Model
public record RefreshTokenQuery(
    string RefreshToken) : IRequest<OperationResult>;