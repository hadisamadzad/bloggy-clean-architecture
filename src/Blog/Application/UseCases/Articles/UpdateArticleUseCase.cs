using Blog.Application.Constants;
using Blog.Application.Helpers;
using Blog.Application.Interfaces;
using Blog.Application.Types.Models.Articles;
using Common.Helpers;
using Common.Utilities.OperationResult;
using FluentValidation;
using MediatR;

namespace Blog.Application.UseCases.Articles;

// Handler
public class UpdateArticleHandler(IRepositoryManager repository) :
    IRequestHandler<UpdateArticleCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var validation = new UpdateArticleValidator().Validate(request);
        if (!validation.IsValid)
            return OperationResult.Failure(OperationStatus.Invalid, validation.GetFirstError());

        // Check duplicate
        var existingSlug = await repository.Articles.GetBySlugAsync(request.Slug);
        if (existingSlug is not null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.DuplicateArticle);

        var entity = await repository.Articles.GetByIdAsync(request.ArticleId);

        entity.Title = request.Title;
        entity.Subtitle = request.Subtitle;
        entity.Summary = request.Summary;
        entity.Content = request.Content;
        entity.Slug = request.Slug.ToLower();
        entity.ThumbnailUrl = request.ThumbnailUrl;
        entity.CoverImageUrl = request.CoverImageUrl;

        entity.TimeToReadInMinute = request.TimeToRead;
        entity.TagIds = [.. request.TagIds];
        entity.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Articles.UpdateAsync(entity);

        return OperationResult.Success(entity.MapToModel());
    }
}

// Model
public record UpdateArticleCommand : IRequest<OperationResult>
{
    public string ArticleId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public int TimeToRead { get; set; }
    public ICollection<string> TagIds { get; set; } = [];
}

// Model Validator
public class UpdateArticleValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleValidator()
    {
        // ArticleId
        RuleFor(x => x.ArticleId)
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
            .NotEmpty()
            .MaximumLength(100)
            .Must(x => SlugHelper.IsValidSlug(x))
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