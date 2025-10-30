using Bloggy.Blog.Application.Helpers;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using FluentValidation;

namespace Bloggy.Blog.Application.Operations.Tags;

public class UpdateTagOperation(IRepositoryManager repository) :
    IOperation<UpdateTagCommand, NoResult>
{
    public async Task<OperationResult<NoResult>> ExecuteAsync(
        UpdateTagCommand command, CancellationToken? cancellation = null)
    {
        // Validate
        var validation = new UpdateTagValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult.ValidationFailure([.. validation.GetErrorMessages()]);

        var entity = await repository.Tags.GetByIdAsync(command.TagId);
        if (entity is null)
            return OperationResult.NotFoundFailure("Tag not found.");

        var slug = string.IsNullOrWhiteSpace(command.Slug) ?
            SlugHelper.GenerateSlug(command.Name) : command.Slug.ToLower();

        // Check duplicate slug (exclude current tag)
        var existing = await repository.Tags.GetBySlugAsync(slug);
        if (existing is not null && existing.Id != command.TagId)
            return OperationResult.Failure("Slug already in use.");

        entity.Name = command.Name;
        entity.Slug = slug;
        entity.UpdatedAt = DateTime.UtcNow;

        var updated = await repository.Tags.UpdateAsync(entity);
        if (!updated)
            return OperationResult.Failure("Failed to update tag.");

        return OperationResult.Success();
    }
}

public record UpdateTagCommand(string TagId, string Name, string Slug) : IOperationCommand;

public class UpdateTagValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagValidator()
    {
        RuleFor(x => x.TagId).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(35);
    }
}
