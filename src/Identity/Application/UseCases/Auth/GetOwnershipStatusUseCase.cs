using Common.Utilities.OperationResult;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.UseCases.Auth;

// Handler
internal class GetOwnershipStatusHandler(IRepositoryManager repository)
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