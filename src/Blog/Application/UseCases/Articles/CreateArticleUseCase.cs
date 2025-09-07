using Blog.Application.Constants;
using Blog.Application.Helpers;
using Blog.Application.Interfaces;
using Blog.Application.Types.Entities;
using Blog.Application.Types.Models.Articles;
using Common.Helpers;
using Common.Utilities.OperationResult;
using FluentValidation;
using MediatR;

namespace Blog.Application.UseCases.Articles;

// Handler
public class CreateArticleHandler(IRepositoryManager repository) :
    IRequestHandler<CreateArticleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validation = new CreateArticleValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        var slug = string.IsNullOrWhiteSpace(request.Slug) ?
            SlugHelper.GenerateSlug(request.Title) : request.Slug;

        // Check duplicate
        var existingSlug = await repository.Articles.GetBySlugAsync(request.Slug);
        if (existingSlug is not null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.DuplicateArticle);

        var entity = new ArticleEntity
        {
            Id = UidHelper.GenerateNewId("article"),
            AuthorId = request.AuthorId,

            Title = request.Title,
            Subtitle = request.Subtitle,
            Summary = request.Summary,
            Content = request.Content,
            Slug = slug,
            ThumbnailUrl = request.ThumbnailUrl,
            CoverImageUrl = request.CoverImageUrl,

            TimeToReadInMinute = 6,
            TagIds = [.. request.TagIds],

            Status = ArticleStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await repository.Articles.InsertAsync(entity);

        return OperationResult.Success(entity.MapToModel());
    }
}

// Model
public record CreateArticleCommand : IRequest<OperationResult>
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

// Model Validator
public class CreateArticleValidator : AbstractValidator<CreateArticleCommand>
{
    public CreateArticleValidator()
    {
        // AuthorId
        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithState(_ => Errors.InvalidId);

        // Title
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithState(_ => Errors.InvalidArticleTitle);

        // Subtitle
        RuleFor(x => x.Subtitle)
            .MaximumLength(300)
            .When(x => !string.IsNullOrEmpty(x.Subtitle))
            .WithState(_ => Errors.InvalidArticleTitle);

        // Summary
        RuleFor(x => x.Summary)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Summary))
            .WithState(_ => Errors.InvalidArticleSummary);

        // Slug
        RuleFor(x => x.Slug)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Slug))
            .WithState(_ => Errors.InvalidSlug);

        // ThumbnailUrl
        RuleFor(x => x.ThumbnailUrl)
            .MaximumLength(300)
            .When(x => !string.IsNullOrEmpty(x.ThumbnailUrl))
            .WithState(_ => Errors.InvalidArticleThumbnailUrl);

        // CoverImageUrl
        RuleFor(x => x.CoverImageUrl)
            .MaximumLength(300)
            .When(x => !string.IsNullOrEmpty(x.CoverImageUrl))
            .WithState(_ => Errors.InvalidArticleCoverImageUrl);

        // TagIds
        RuleForEach(x => x.TagIds)
            .NotEmpty()
            .WithState(_ => Errors.InvalidId);
    }
}