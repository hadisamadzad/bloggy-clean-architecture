using Bloggy.Blog.Application.Operations.Articles;
using Bloggy.Blog.Application.Operations.Settings;
using Bloggy.Blog.Application.Operations.Subscribers;
using Bloggy.Blog.Application.Operations.Tags;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Blog.Application.Types.Models.Settings;
using Bloggy.Blog.Application.Types.Models.Tags;
using Bloggy.Core.Utilities.OperationResult;
using Bloggy.Core.Utilities.Pagination;

namespace Bloggy.Blog.Application.Interfaces;

public interface IOperationService
{
    // Article
    IOperation<CreateArticleCommand, string> CreateArticle { get; }
    IOperation<DeleteArticleCommand, NoResult> DeleteArticle { get; }
    IOperation<GetArticleByIdCommand, ArticleModel> GetArticleById { get; }
    IOperation<GetPublishedArticleBySlugCommand, ArticleModel> GetArticleBySlug { get; }
    IOperation<GetArticlesByFilterCommand, PaginatedList<ArticleModel>> GetArticlesByFilter { get; }
    IOperation<UpdateArticleCommand, NoResult> UpdateArticle { get; }
    IOperation<UpdateArticleStatusCommand, NoResult> UpdateArticleStatus { get; }

    // Tags
    IOperation<CreateTagCommand, string> CreateTag { get; }
    IOperation<UpdateTagCommand, NoResult> UpdateTag { get; }
    IOperation<DeleteTagCommand, NoResult> DeleteTag { get; }
    IOperation<GetAllTagsCommand, List<TagModel>> GetAllTags { get; }

    // Settings
    IOperation<GetBlogSettingsCommand, SettingModel> GetBlogSettings { get; }
    IOperation<UpdateBlogSettingsCommand, NoResult> UpdateBlogSettings { get; }

    // Subscriber
    IOperation<CreateSubscriberCommand, string> CreateSubscriber { get; }
    IOperation<DeleteSubscriberCommand, NoResult> DeleteSubscriber { get; }
}