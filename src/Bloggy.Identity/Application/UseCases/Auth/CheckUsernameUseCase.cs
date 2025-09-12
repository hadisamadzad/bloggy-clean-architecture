using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Identity.Application.Constants;
using Bloggy.Identity.Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Bloggy.Identity.Application.UseCases.Auth;

// Handler
public class CheckUsernameHandler(IRepositoryManager repository) :
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