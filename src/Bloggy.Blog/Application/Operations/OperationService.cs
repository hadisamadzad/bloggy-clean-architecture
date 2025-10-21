using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Operations.Articles;
using Bloggy.Blog.Application.Operations.Settings;
using Bloggy.Blog.Application.Operations.Subscribers;
using Bloggy.Blog.Application.Operations.Tags;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Blog.Application.Types.Models.Settings;
using Bloggy.Blog.Application.Types.Models.Tags;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Core.Utilities.Pagination;

namespace Bloggy.Blog.Application.Operations;

#pragma warning disable S107 // Avoid excessive complexity
public class OperationService(
    // Articles
    IOperation<CreateArticleCommand, string> createArticle,
    IOperation<DeleteArticleCommand, NoResult> deleteArticle,
    IOperation<GetArticleByIdCommand, ArticleModel> getArticleById,
    IOperation<GetArticleBySlugCommand, ArticleModel> getArticleBySlug,
    IOperation<GetArticlesByFilterCommand, PaginatedList<ArticleModel>> getArticlesByFilter,
    IOperation<UpdateArticleCommand, NoResult> updateArticle,
    IOperation<UpdateArticleStatusCommand, NoResult> updateArticleStatus,

    // Tags
    IOperation<CreateTagCommand, string> createTag,
    IOperation<GetAllTagsCommand, List<TagModel>> getAllTags,

    // Settings
    IOperation<GetBlogSettingsCommand, SettingModel> getBlogSettings,
    IOperation<UpdateBlogSettingsCommand, NoResult> updateBlogSettings,

    // Subscribers
    IOperation<CreateSubscriberCommand, string> createSubscriber,
    IOperation<DeleteSubscriberCommand, NoResult> deleteSubscriber
    ) : IOperationService
#pragma warning restore S107
{
    // Article
    public IOperation<CreateArticleCommand, string> CreateArticle { get; } = createArticle;
    public IOperation<DeleteArticleCommand, NoResult> DeleteArticle { get; } = deleteArticle;
    public IOperation<GetArticleByIdCommand, ArticleModel> GetArticleById { get; } = getArticleById;
    public IOperation<GetArticleBySlugCommand, ArticleModel> GetArticleBySlug { get; } = getArticleBySlug;
    public IOperation<GetArticlesByFilterCommand, PaginatedList<ArticleModel>> GetArticlesByFilter { get; } = getArticlesByFilter;
    public IOperation<UpdateArticleCommand, NoResult> UpdateArticle { get; } = updateArticle;
    public IOperation<UpdateArticleStatusCommand, NoResult> UpdateArticleStatus { get; } = updateArticleStatus;

    // Tags
    public IOperation<CreateTagCommand, string> CreateTag { get; } = createTag;
    public IOperation<GetAllTagsCommand, List<TagModel>> GetAllTags { get; } = getAllTags;

    // Settings
    public IOperation<GetBlogSettingsCommand, SettingModel> GetBlogSettings { get; } = getBlogSettings;
    public IOperation<UpdateBlogSettingsCommand, NoResult> UpdateBlogSettings { get; } = updateBlogSettings;

    // Subscriber
    public IOperation<CreateSubscriberCommand, string> CreateSubscriber { get; } = createSubscriber;
    public IOperation<DeleteSubscriberCommand, NoResult> DeleteSubscriber { get; } = deleteSubscriber;

}