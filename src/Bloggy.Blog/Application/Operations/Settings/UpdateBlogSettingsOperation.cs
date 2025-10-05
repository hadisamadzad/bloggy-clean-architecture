using Bloggy.Blog.Application.Constants;
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

        entity.BlogTitle = command.BlogTitle;
        entity.BlogSubtitle = command.BlogSubtitle;
        entity.BlogPageTitle = command.BlogPageTitle;
        entity.BlogDescription = command.BlogDescription;
        entity.SeoMetaTitle = command.SeoMetaTitle;
        entity.SeoMetaDescription = command.SeoMetaDescription;
        entity.BlogUrl = command.BlogUrl;
        entity.BlogLogoUrl = command.BlogLogoUrl;
        entity.Socials = command.Socials;

        entity.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Settings.UpdateAsync(entity);

        return OperationResult.Success();
    }
}

public record UpdateBlogSettingsCommand() : IOperationCommand
{
    public required string BlogTitle { get; set; }
    public required string BlogSubtitle { get; set; }
    public required string BlogPageTitle { get; set; }
    public required string BlogDescription { get; set; } = string.Empty;
    public string SeoMetaTitle { get; set; } = string.Empty;
    public string SeoMetaDescription { get; set; } = string.Empty;
    public string BlogUrl { get; set; } = string.Empty;
    public string BlogLogoUrl { get; set; } = string.Empty;
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
            .WithState(_ => Errors.InvalidBlogTitle)
            .MaximumLength(100)
            .WithState(_ => Errors.InvalidBlogTitle);

        // BlogSubtitle
        RuleFor(x => x.BlogSubtitle)
            .NotEmpty()
            .WithState(_ => Errors.InvalidBlogTitle)
            .MaximumLength(200)
            .WithState(_ => Errors.InvalidBlogTitle);

        // BlogPageTitle
        RuleFor(x => x.BlogPageTitle)
            .NotEmpty()
            .WithState(_ => Errors.InvalidBlogTitle)
            .MaximumLength(150)
            .WithState(_ => Errors.InvalidBlogTitle);

        // BlogDescription
        RuleFor(x => x.BlogDescription)
            .NotEmpty()
            .WithState(_ => Errors.InvalidBlogDescription)
            .MaximumLength(500)
            .WithState(_ => Errors.InvalidBlogDescription);

        // SeoMetaTitle
        RuleFor(x => x.SeoMetaTitle)
            .MaximumLength(60)
            .When(x => !string.IsNullOrEmpty(x.SeoMetaTitle))
            .WithState(_ => Errors.InvalidSeoTitle);

        // SeoMetaDescription
        RuleFor(x => x.SeoMetaDescription)
            .MaximumLength(160)
            .When(x => !string.IsNullOrEmpty(x.SeoMetaDescription))
            .WithState(_ => Errors.InvalidSeoDescription);

        // BlogUrl
        RuleFor(x => x.BlogUrl)
            .NotEmpty()
            .WithState(_ => Errors.InvalidBlogUrl);

        // Socials
        RuleForEach(x => x.Socials)
            .Must(x => Enum.IsDefined(x.Name))
            .WithState(_ => Errors.InvalidSocialNetworkName);
    }
}