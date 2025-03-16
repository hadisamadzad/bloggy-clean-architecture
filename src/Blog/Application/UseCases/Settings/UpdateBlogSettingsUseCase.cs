using Blog.Application.Constants;
using Blog.Application.Interfaces;
using Blog.Application.Types.Entities;
using Common.Utilities.OperationResult;
using FluentValidation;
using MediatR;

namespace Blog.Application.UseCases.Settings;

// Handler
internal class UpdateBlogSettingsHandler(IRepositoryManager repository) :
    IRequestHandler<UpdateBlogSettingsCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateBlogSettingsCommand request, CancellationToken cancellationToken)
    {
        // Retrieve the article
        var entity = await repository.Settings.GetBlogSettingAsync();
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.SettingsNotFound);

        entity.BlogTitle = request.BlogTitle;
        entity.BlogDescription = request.BlogDescription;
        entity.SeoMetaTitle = request.SeoMetaTitle;
        entity.SeoMetaDescription = request.SeoMetaDescription;
        entity.BlogUrl = request.BlogUrl;
        entity.BlogLogoUrl = request.BlogLogoUrl;
        entity.Socials = request.Socials;

        entity.UpdatedAt = DateTime.UtcNow;

        _ = await repository.Settings.UpdateAsync(entity);

        return OperationResult.Success(entity);
    }
}

// Model
public record UpdateBlogSettingsCommand() : IRequest<OperationResult>
{
    public required string BlogTitle { get; set; }
    public required string BlogDescription { get; set; } = string.Empty;
    public string SeoMetaTitle { get; set; } = string.Empty;
    public string SeoMetaDescription { get; set; } = string.Empty;
    public string BlogUrl { get; set; } = string.Empty;
    public string BlogLogoUrl { get; set; } = string.Empty;
    public ICollection<SocialNetwork> Socials { get; set; } = [];
}

// Model Validator
public class UpdateBlogSettingsValidator : AbstractValidator<UpdateBlogSettingsCommand>
{
    public UpdateBlogSettingsValidator()
    {
        // BlogTitle
        RuleFor(x => x.BlogTitle)
            .NotEmpty()
            .MaximumLength(100)
            .WithState(_ => Errors.InvalidBlogTitle);

        // BlogDescription
        RuleFor(x => x.BlogDescription)
            .NotEmpty()
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

        // BlogLogoUrl
        RuleFor(x => x.BlogLogoUrl)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.BlogLogoUrl))
            .WithState(_ => Errors.InvalidBlogLogoUrl);

        // Socials
        RuleForEach(x => x.Socials)
            .Must(x => Enum.IsDefined(x.Name))
            .WithState(_ => Errors.InvalidSocialNetworkName);
    }
}