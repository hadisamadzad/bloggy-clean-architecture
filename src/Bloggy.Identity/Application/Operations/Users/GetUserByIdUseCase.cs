using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Constants;
using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Types.Models.Users;
using MediatR;

namespace Bloggy.Identity.Application.UseCases.Users;

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