using Common.Utilities.OperationResult;
using Identity.Application.Constants;
using Identity.Application.Interfaces;
using Identity.Application.Types.Models.Users;
using MediatR;

namespace Identity.Application.UseCases.Auth;

// Handler
public class GetUserProfileHandler(IRepositoryManager repository) :
    IRequestHandler<GetUserProfileQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.RequestedBy))
            return OperationResult.Failure(OperationStatus.Invalid, Errors.InvalidId);

        // Get
        var user = await repository.Users.GetByIdAsync(request.RequestedBy);
        if (user is null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.InvalidId);

        // Mapping
        var response = user.MapToUserModel();

        return OperationResult.Success(response);
    }
}

// Model
public record GetUserProfileQuery(
    string RequestedBy) : IRequest<OperationResult>;