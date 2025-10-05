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
                        BlogTitle = operationResult.Value!.BlogTitle,
                        BlogSubtitle = operationResult.Value!.BlogSubtitle,
                        BlogPageTitle = operationResult.Value!.BlogPageTitle,
                        BlogDescription = operationResult.Value!.BlogDescription,
                        SeoMetaTitle = operationResult.Value!.SeoMetaTitle,
                        SeoMetaDescription = operationResult.Value!.SeoMetaDescription,
                        BlogUrl = operationResult.Value!.BlogUrl,
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
    public required string BlogTitle { get; set; }
    public required string BlogSubtitle { get; set; }
    public required string BlogPageTitle { get; set; }
    public required string BlogDescription { get; set; } = string.Empty;
    public string SeoMetaTitle { get; set; } = string.Empty;
    public string SeoMetaDescription { get; set; } = string.Empty;
    public string BlogUrl { get; set; } = string.Empty;
    public string BlogLogoUrl { get; set; } = string.Empty;
    public ICollection<SocialNetwork> Socials { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
};