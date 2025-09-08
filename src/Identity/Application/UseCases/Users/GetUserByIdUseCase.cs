using Common.Utilities.OperationResult;
using Identity.Application.Constants;
using Identity.Application.Interfaces;
using Identity.Application.Types.Models.Users;
using MediatR;

namespace Identity.Application.UseCases.Users;

// Handler
public class GetUserByIdHandler(IRepositoryManager repository) :
    IRequestHandler<GetUserByIdQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        // Get
        var entity = await repository.Users.GetByIdAsync(request.UserId);
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.InvalidId);

        // Mapping
        var model = entity.MapToUserModel();

        return OperationResult.Success(model);
    }
}

// Model
public record GetUserByIdQuery(string UserId) : IRequest<OperationResult>;