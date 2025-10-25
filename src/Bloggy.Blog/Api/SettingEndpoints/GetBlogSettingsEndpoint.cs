using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Settings;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;

namespace Bloggy.Blog.Api.SettingEndpoints;

public class GetBlogSettingsEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for getting blog settings
        app.MapGroup(Routes.SettingBaseRoute)
            .WithSummary("Get Blog Settings")
            .MapGet("/", async (IOperationService operations) =>
            {
                // Operation
                var operationResult = await operations.GetBlogSettings.ExecuteAsync(new GetBlogSettingsCommand());

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.Ok(new GetBlogSettingsResponse
                    {
                        AuthorName = operationResult.Value!.AuthorName,
                        AuthorTitle = operationResult.Value!.AuthorTitle,
                        AboutAuthor = operationResult.Value!.AboutAuthor,
                        BlogTitle = operationResult.Value!.BlogTitle,
                        BlogSubtitle = operationResult.Value!.BlogSubtitle,
                        BlogDescription = operationResult.Value!.BlogDescription,
                        BlogUrl = operationResult.Value!.BlogUrl,
                        PageTitleTemplate = operationResult.Value!.PageTitleTemplate,
                        SeoMetaTitle = operationResult.Value!.SeoMetaTitle,
                        SeoMetaDescription = operationResult.Value!.SeoMetaDescription,
                        CopyrightText = operationResult.Value!.CopyrightText,
                        BlogLogoUrl = operationResult.Value!.BlogLogoUrl,
                        Socials = [.. operationResult.Value!.Socials
                            .Where(x => !string.IsNullOrEmpty(x.Url))
                            .OrderBy(x => x.Order).Select(x => new SocialNetwork
                            {
                                Order = x.Order,
                                Name = x.Name,
                                Url = x.Url
                            })],
                        UpdatedAt = operationResult.Value!.UpdatedAt,
                    }),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.SettingEndpointGroupTag)
            .WithDescription("Gets the current blog settings.")
            .Produces<GetBlogSettingsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record GetBlogSettingsResponse
{
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorTitle { get; set; } = string.Empty;
    public string AboutAuthor { get; set; } = string.Empty;
    public required string BlogTitle { get; set; }
    public required string BlogSubtitle { get; set; }
    public string BlogDescription { get; set; } = string.Empty;
    public string BlogUrl { get; set; } = string.Empty;
    public required string PageTitleTemplate { get; set; }
    public string SeoMetaTitle { get; set; } = string.Empty;
    public string SeoMetaDescription { get; set; } = string.Empty;
    public string BlogLogoUrl { get; set; } = string.Empty;
    public string CopyrightText { get; set; } = string.Empty;
    public ICollection<SocialNetwork> Socials { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
};