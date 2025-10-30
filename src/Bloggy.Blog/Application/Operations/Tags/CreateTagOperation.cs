using Bloggy.Blog.Application.Helpers;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using FluentValidation;

namespace Bloggy.Blog.Application.Operations.Tags;

public class CreateTagOperation(IRepositoryManager repository) :
    IOperation<CreateTagCommand, string>
{
    public async Task<OperationResult<string>> ExecuteAsync(
        CreateTagCommand command, CancellationToken? cancellation = null)
    {
        // Validate
        var validation = new CreateTagValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult<string>.ValidationFailure([.. validation.GetErrorMessages()]);

        var slug = string.IsNullOrWhiteSpace(command.Slug) ?
            SlugHelper.GenerateSlug(command.Name) : command.Slug.ToLower();

        // Check duplicate
        var existingSlug = await repository.Tags.GetBySlugAsync(slug);
        if (existingSlug is not null)
            return OperationResult<string>.Failure("Slug already in use.");

        var entity = new TagEntity
        {
            Id = UidHelper.GenerateNewId("tag"),
            Name = command.Name,
            Slug = slug,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.Tags.InsertAsync(entity);

        return OperationResult<string>.Success(entity.Id);
    }
}

public record CreateTagCommand(string Name, string Slug) : IOperationCommand;

// Validator
public class CreateTagValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagValidator()
    {
        // Name
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(30);

        // Slug
        RuleFor(x => x.Slug)
            .MaximumLength(30)
            .When(x => !string.IsNullOrEmpty(x.Slug));
    }
}