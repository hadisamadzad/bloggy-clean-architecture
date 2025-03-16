using Blog.Application.Constants;
using Blog.Application.Helpers;
using Blog.Application.Interfaces;
using Blog.Application.Types.Entities;
using Common.Helpers;
using Common.Utilities.OperationResult;
using FluentValidation;
using MediatR;

namespace Blog.Application.UseCases.Tags;

// Handler
internal class CreateTagHandler(IRepositoryManager repository) :
    IRequestHandler<CreateTagCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validation = new CreateTagValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        var slug = string.IsNullOrWhiteSpace(request.Slug) ?
            SlugHelper.GenerateSlug(request.Name) : request.Slug;

        // Check duplicate
        var existingSlug = await repository.Tags.GetBySlugAsync(slug);
        if (existingSlug is not null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.DuplicateTag);

        var entity = new TagEntity
        {
            Id = UidHelper.GenerateNewId("tag"),
            Name = request.Name,
            Slug = slug,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.Tags.InsertAsync(entity);

        return OperationResult.Success(entity);
    }
}

// Model
public record CreateTagCommand(string Name, string Slug) : IRequest<OperationResult>;

// Model Validator
public class CreateTagValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagValidator()
    {
        // Name
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(30)
            .WithState(_ => Errors.InvalidTagName);

        // Slug
        RuleFor(x => x.Slug)
            .MaximumLength(30)
            .When(x => !string.IsNullOrEmpty(x.Slug))
            .WithState(_ => Errors.InvalidSlug);
    }
}