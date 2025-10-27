using Bloggy.Blog.Application.Helpers;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using FluentValidation;

namespace Bloggy.Blog.Application.Operations.Articles;

public class CreateArticleOperation(IRepositoryManager repository) :
    IOperation<CreateArticleCommand, string>
{
    public async Task<OperationResult<string>> ExecuteAsync(
        CreateArticleCommand command, CancellationToken? cancellation = null)
    {
        // Validate
        var validation = new CreateArticleValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult<string>.ValidationFailure([.. validation.GetErrorMessages()]);

        var slug = string.IsNullOrWhiteSpace(command.Slug) ?
            SlugHelper.GenerateSlug(command.Title) : command.Slug;

        // Check duplicate
        var existingSlug = await repository.Articles.GetBySlugAsync(command.Slug);
        if (existingSlug is not null)
            return OperationResult<string>.Failure("Slug is already in use.");

        var words = command.Content?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length ?? 0;
        var timeToRead = (int)Math.Ceiling(words / 150.0); // 150 words per minute
        var entity = new ArticleEntity
        {
            Id = UidHelper.GenerateNewId("article"),
            AuthorId = command.AuthorId,
            Title = command.Title,
            Subtitle = command.Subtitle,
            Summary = command.Summary,
            Content = command.Content,
            Slug = slug,
            ThumbnailUrl = command.ThumbnailUrl,
            CoverImageUrl = command.CoverImageUrl,
            OriginalArticleInfo = command.OriginalArticleInfo,
            TimeToReadInMinute = timeToRead,
            TagIds = [.. command.TagIds],
            Status = ArticleStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.Articles.InsertAsync(entity);

        return OperationResult<string>.Success(entity.Slug);
    }
}

public record CreateArticleCommand : IOperationCommand
{
    public required string AuthorId { get; init; }
    public required string Title { get; init; }
    public required string Subtitle { get; init; }
    public required string Summary { get; init; }
    public required string Content { get; init; }
    public required string Slug { get; init; }
    public required string ThumbnailUrl { get; init; }
    public required string CoverImageUrl { get; init; }
    public required OriginalArticleInfoValue? OriginalArticleInfo { get; init; }
    public required ICollection<string> TagIds { get; init; } = [];
}

// Validator
public class CreateArticleValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleValidator()
    {
        // AuthorId
        RuleFor(x => x.AuthorId)
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
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Slug));

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