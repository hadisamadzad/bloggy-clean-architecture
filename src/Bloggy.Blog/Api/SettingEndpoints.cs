using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.UseCases.Settings;
using Bloggy.Core.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bloggy.Blog.Api;

public class SettingEndpoints : IEndpoint
{
    const string Route = "api/settings/";
    const string Tag = "Settings";

    // Endpoints
    public void MapEndpoints(WebApplication app)
    {
        var group = app.MapGroup(Route).WithTags(Tag);

        // Endpoint for getting blog settings
        group.MapGet("/", async (
            IMediator mediator) =>
            {
                return await mediator.Send(new GetBlogSettingsQuery());
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (SettingEntity)operation.Value;
                return new
                {
                    SettingId = value.Id,
                    BlogTitle = value.BlogTitle,
                    BlogDescription = value.BlogDescription,
                    SeoMetaTitle = value.SeoMetaTitle,
                    SeoMetaDescription = value.SeoMetaDescription,
                    BlogUrl = value.BlogUrl,
                    BlogLogoUrl = value.BlogLogoUrl,
                    Socials = value.Socials
                        .Where(x => !string.IsNullOrEmpty(x.Url))
                        .OrderBy(x => x.Order).Select(x => new
                        {
                            x.Name,
                            x.Url
                        }),
                    UpdatedAt = value.UpdatedAt,
                };
            });

        // Endpoint for updating blog settings
        group.MapPut("/", async (
            IMediator mediator,
            [FromBody] UpdateBlogSettingsRequest request) =>
            {
                return await mediator.Send(new UpdateBlogSettingsCommand
                {
                    BlogTitle = request.BlogTitle,
                    BlogDescription = request.BlogDescription,
                    SeoMetaTitle = request.SeoMetaTitle,
                    SeoMetaDescription = request.SeoMetaDescription,
                    BlogUrl = request.BlogUrl,
                    BlogLogoUrl = request.BlogLogoUrl,
                    Socials = request.Socials,
                });
            })
            .AddEndpointFilter(async (context, next) =>
            {
                var operation = await next(context) as OperationResult;
                if (!operation!.Succeeded)
                    return operation.GetHttpResult();

                var value = (SettingEntity)operation.Value;
                return new
                {
                    SettingId = value.Id,
                };
            });
    }
}

// Models
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