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

            TimeToReadInMinute = 6, // FIXME: Calculate time to read
            TagIds = [.. command.TagIds],

            Status = ArticleStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.Articles.InsertAsync(entity);

        return OperationResult<string>.Success(entity.Id);
    }
}

public record CreateArticleCommand : IOperationCommand
{
    public required string AuthorId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public ICollection<string> TagIds { get; set; } = [];
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