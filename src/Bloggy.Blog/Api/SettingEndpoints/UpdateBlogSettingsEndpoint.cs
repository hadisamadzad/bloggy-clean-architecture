using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Settings;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api.SettingEndpoints;

public class UpdateBlogSettingsEndpoint : IEndpoint
{
    public void MapEndpoints(WebApplication app)
    {
        // Endpoint for updating blog settings
        app.MapGroup(Routes.SettingBaseRoute)
            .WithSummary("Update Blog Settings")
            .MapPut("/", async (IOperationService operations,
                [FromBody] UpdateBlogSettingsRequest request) =>
            {
                // Operation
                var operationResult = await operations.UpdateBlogSettings.ExecuteAsync(
                    new UpdateBlogSettingsCommand
                    {
                        BlogTitle = request.BlogTitle,
                        BlogDescription = request.BlogDescription,
                        SeoMetaTitle = request.SeoMetaTitle,
                        SeoMetaDescription = request.SeoMetaDescription,
                        BlogUrl = request.BlogUrl,
                        BlogLogoUrl = request.BlogLogoUrl,
                        Socials = request.Socials,
                    });

                // Result
                return operationResult.Status switch
                {
                    OperationStatus.Completed => Results.NoContent(),
                    OperationStatus.Invalid => Results.BadRequest(operationResult.Error),
                    OperationStatus.NotFound => Results.UnprocessableEntity(operationResult.Error),
                    _ => Results.InternalServerError(operationResult.Error),
                };
            })
            .WithTags(Routes.SettingEndpointGroupTag)
            .WithDescription("Updates the blog settings.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}

public record UpdateBlogSettingsRequest
{
    public required string BlogTitle { get; set; }
    public required string BlogDescription { get; set; } = string.Empty;
    public string SeoMetaTitle { get; set; } = string.Empty;
    public string SeoMetaDescription { get; set; } = string.Empty;
    public string BlogUrl { get; set; } = string.Empty;
    public string BlogLogoUrl { get; set; } = string.Empty;
    public ICollection<SocialNetwork> Socials { get; set; } = [];
};