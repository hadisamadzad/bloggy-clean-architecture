using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Interfaces;
using MediatR;

namespace Bloggy.Identity.Application.UseCases.Auth;

// Handler
public class GetOwnershipStatusHandler(IRepositoryManager repository)
    : IRequestHandler<GetOwnershipStatusQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetOwnershipStatusQuery request, CancellationToken cancellationToken)
    {
        var isAlreadyOwned = await repository.Users.AnyAsync();

        return OperationResult.Success(isAlreadyOwned);
    }
}

// Model
public record GetOwnershipStatusQuery() : IRequest<OperationResult>;