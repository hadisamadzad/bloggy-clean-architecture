using Common.Helpers;
using Common.Utilities.OperationResult;
using FluentValidation;
using Identity.Application.Constants;
using Identity.Application.Interfaces;
using MediatR;

namespace Identity.Application.UseCases.Auth;

// Handler
internal class CheckUsernameHandler(IRepositoryManager repository) :
    IRequestHandler<CheckUsernameQuery, OperationResult>
{
    public async Task<OperationResult> Handle(CheckUsernameQuery request, CancellationToken cancellationToken)
    {
        // Validation
        var validation = new CheckUsernameValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        // Get
        var user = await repository.Users.GetByEmailAsync(request.Email);

        var isAvailable = user is null;

        return OperationResult.Success(isAvailable);
    }
}

// Model
public record CheckUsernameQuery(string Email) : IRequest<OperationResult>;

// Model Validator
public class CheckUsernameValidator : AbstractValidator<CheckUsernameQuery>
{
    public CheckUsernameValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithState(_ => Errors.InvalidEmail);
    }
}