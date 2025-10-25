using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using FluentValidation;

namespace Bloggy.Blog.Application.Operations.Settings;

public class UpdateBlogSettingsOperation(IRepositoryManager repository) :
    IOperation<UpdateBlogSettingsCommand, NoResult>
{
    public async Task<OperationResult<NoResult>> ExecuteAsync(
        UpdateBlogSettingsCommand command, CancellationToken? cancellation = null)
    {
        // Validate request
        var validation = new UpdateBlogSettingsValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult.ValidationFailure([.. validation.GetErrorMessages()]);

        // Retrieve the article
        var entity = await repository.Settings.GetBlogSettingAsync();
        if (entity is null)
            return OperationResult.NotFoundFailure("Blog settings not found.");

        entity.AuthorName = command.AuthorName;
        entity.AuthorTitle = command.AuthorTitle;
        entity.AboutAuthor = command.AboutAuthor;
        entity.BlogTitle = command.BlogTitle;
        entity.BlogSubtitle = command.BlogSubtitle;
        entity.BlogDescription = command.BlogDescription;
        entity.BlogUrl = command.BlogUrl;
        entity.PageTitleTemplate = command.PageTitleTemplate;
        entity.CopyrightText = command.CopyrightText;
        entity.SeoMetaTitle = command.SeoMetaTitle;
        entity.SeoMetaDescription = command.SeoMetaDescription;
        entity.BlogLogoUrl = command.BlogLogoUrl;
        entity.Socials = command.Socials;

        entity.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Settings.UpdateAsync(entity);

        return OperationResult.Success();
    }
}

public record UpdateBlogSettingsCommand() : IOperationCommand
{
    // Author
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorTitle { get; set; } = string.Empty;
    public string AboutAuthor { get; set; } = string.Empty;

    public required string BlogTitle { get; set; }
    public required string BlogSubtitle { get; set; }
    public required string PageTitleTemplate { get; set; }
    public string BlogDescription { get; set; } = string.Empty;
    public string SeoMetaTitle { get; set; } = string.Empty;
    public string SeoMetaDescription { get; set; } = string.Empty;
    public string BlogUrl { get; set; } = string.Empty;
    public string BlogLogoUrl { get; set; } = string.Empty;
    public string CopyrightText { get; set; } = string.Empty;
    public ICollection<SocialNetwork> Socials { get; set; } = [];
}

// Validator
public class UpdateBlogSettingsValidator : AbstractValidator<UpdateBlogSettingsCommand>
{
    public UpdateBlogSettingsValidator()
    {
        // BlogTitle
        RuleFor(x => x.BlogTitle)
            .NotEmpty()
            .MaximumLength(100);

        // AuthorName (optional)
        RuleFor(x => x.AuthorName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.AuthorName));

        // AuthorTitle (optional)
        RuleFor(x => x.AuthorTitle)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.AuthorTitle));

        // AboutAuthor (optional)
        RuleFor(x => x.AboutAuthor)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.AboutAuthor));

        // BlogSubtitle
        RuleFor(x => x.BlogSubtitle)
            .NotEmpty()
            .MaximumLength(200);

        // PageTitleTemplate
        RuleFor(x => x.PageTitleTemplate)
            .NotEmpty()
            .MaximumLength(150);
        // CopyrightText
        RuleFor(x => x.CopyrightText)
            .MaximumLength(200);

        // BlogDescription
        RuleFor(x => x.BlogDescription)
            .NotEmpty()
            .MaximumLength(500);

        // SeoMetaTitle
        RuleFor(x => x.SeoMetaTitle)
            .MaximumLength(60)
            .When(x => !string.IsNullOrEmpty(x.SeoMetaTitle));

        // SeoMetaDescription
        RuleFor(x => x.SeoMetaDescription)
            .MaximumLength(160)
            .When(x => !string.IsNullOrEmpty(x.SeoMetaDescription));

        // BlogUrl
        RuleFor(x => x.BlogUrl)
            .NotEmpty();

        // Socials
        RuleForEach(x => x.Socials)
            .Must(x => Enum.IsDefined(x.Name));
    }
}