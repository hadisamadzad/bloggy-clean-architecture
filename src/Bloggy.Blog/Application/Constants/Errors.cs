using Bloggy.Core.Utilities;

namespace Bloggy.Blog.Application.Constants;

public static class Errors
{
    const string BlogError = "Blog Error";
    const string ArticleError = "Article Error";
    const string TagError = "Tag Error";
    const string SettingError = "Setting Error";

    // Common
    public static readonly ErrorModel InvalidId =
        new("BLCO-100", BlogError, "Invalid ID.");

    public static readonly ErrorModel InvalidSlug =
        new("BLCO-101", BlogError, "Invalid slug.");

    public static readonly ErrorModel SettingsNotFound =
        new("BLCO-102", BlogError, "Settings not found.");

    public static readonly ErrorModel InvalidEmail =
        new("BLCO-103", BlogError, "Invalid email address.");

    // Article
    public static readonly ErrorModel InvalidArticleTitle =
        new("BLAR-100", ArticleError, "Invalid article title.");

    public static readonly ErrorModel InvalidArticleSubtitle =
        new("BLAR-101", ArticleError, "Invalid article subtitle.");

    public static readonly ErrorModel InvalidArticleSummary =
    new("BLAR-102", ArticleError, "Invalid article summary.");

    public static readonly ErrorModel InvalidArticleThumbnailUrl =
        new("BLAR-104", ArticleError, "Invalid article thumbnail URL.");

    public static readonly ErrorModel InvalidArticleCoverImageUrl =
        new("BLAR-105", ArticleError, "Invalid article cover image URL.");

    public static readonly ErrorModel ArticleNotFound =
        new("BLAR-106", ArticleError, "Article not found.");

    public static readonly ErrorModel DuplicateArticle =
        new("BLAR-107", TagError, "Duplicate article.");

    // Tag
    public static readonly ErrorModel InvalidTagName =
        new("BLTG-101", TagError, "Invalid tag name.");

    public static readonly ErrorModel DuplicateTag =
        new("BLTG-102", TagError, "Duplicate tag.");

    // BlogSettings
    public static readonly ErrorModel InvalidBlogTitle =
        new("BLST-101", SettingError, "Invalid blog title.");

    public static readonly ErrorModel InvalidBlogDescription =
        new("BLST-102", SettingError, "Invalid blog description.");

    public static readonly ErrorModel InvalidSeoTitle =
        new("BLST-103", SettingError, "Invalid SEO title.");

    public static readonly ErrorModel InvalidSeoDescription =
        new("BLST-104", SettingError, "Invalid SEO description.");

    public static readonly ErrorModel InvalidBlogUrl =
        new("BLST-105", SettingError, "Invalid blog URL.");

    public static readonly ErrorModel InvalidBlogLogoUrl =
        new("BLST-106", SettingError, "Invalid blog logo URL.");

    public static readonly ErrorModel InvalidSocialNetworkName =
        new("BLST-107", SettingError, "Invalid social network name.");
}