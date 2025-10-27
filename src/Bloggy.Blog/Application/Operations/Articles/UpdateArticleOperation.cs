using Bloggy.Blog.Application.Helpers;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using FluentValidation;

namespace Bloggy.Blog.Application.Operations.Articles;

public class UpdateArticleOperation(IRepositoryManager repository) :
    IOperation<UpdateArticleCommand, NoResult>
{
    public async Task<OperationResult<NoResult>> ExecuteAsync(
        UpdateArticleCommand command, CancellationToken? cancellation = null)
    {
        // Validate
        var validation = new UpdateArticleValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult.ValidationFailure([.. validation.GetErrorMessages()]);

        // Check duplicate
        var existingEntityBySlug = await repository.Articles.GetBySlugAsync(command.Slug);
        if (existingEntityBySlug is not null && existingEntityBySlug.Id != command.ArticleId)
            return OperationResult.Failure("Slug is already in use.");

        var entity = await repository.Articles.GetByIdAsync(command.ArticleId);

        entity.Title = command.Title;
        entity.Subtitle = command.Subtitle;
        entity.Summary = command.Summary;
        entity.Content = command.Content;
        entity.Slug = command.Slug.ToLower();
        entity.ThumbnailUrl = command.ThumbnailUrl;
        entity.CoverImageUrl = command.CoverImageUrl;
        entity.OriginalArticleInfo = command.OriginalArticleInfo;
        entity.TimeToReadInMinute = command.TimeToRead;
        entity.TagIds = [.. command.TagIds];
        entity.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Articles.UpdateAsync(entity);

        return OperationResult.Success();
    }
}

public record UpdateArticleCommand : IOperationCommand
{
    public string ArticleId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string ThumbnailUrl { get; init; } = string.Empty;
    public string CoverImageUrl { get; init; } = string.Empty;
    public OriginalArticleInfoValue? OriginalArticleInfo { get; init; }
    public int TimeToRead { get; init; }
    public ICollection<string> TagIds { get; init; } = [];
}

// Validator
public class UpdateArticleValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleValidator()
    {
        // ArticleId
        RuleFor(x => x.ArticleId)
            .NotEmpty();

        // Title
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        // Subtitle
        RuleFor(x => x.Subtitle)
            .MaximumLength(300)
            .When(x => !string.IsNullOrEmpty(x.Subtitle));

        // Summary
        RuleFor(x => x.Summary)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Summary));

        // Slug
        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100)
            .Must(x => SlugHelper.IsValidSlug(x));

        // ThumbnailUrl
        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(300)
            .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl));

        // CoverImageUrl
        RuleFor(x => x.CoverImageUrl)
            .MaximumLength(300)
            .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));

        // TagIds
        RuleForEach(x => x.TagIds)
            .NotEmpty();
    }
}