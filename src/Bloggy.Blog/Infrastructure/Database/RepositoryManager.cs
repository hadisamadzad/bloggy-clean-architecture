using Blog.Infrastructure.Database.Repositories;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Interfaces.Repositories;
using MongoDB.Driver;

namespace Bloggy.Blog.Infrastructure.Database;

public class RepositoryManager(IMongoDatabase mongoDatabase) : IRepositoryManager
{
    public IArticleRepository Articles { get; } =
        new ArticleRepository(mongoDatabase, "blog.articles");

    public ITagRepository Tags { get; } =
        new TagRepository(mongoDatabase, "blog.tags");

    public ISubscriberRepository Subscribers { get; } =
        new SubscriberRepository(mongoDatabase, "blog.subscribers");

    public ISettingRepository Settings { get; } =
        new SettingRepository(mongoDatabase, "blog.settings");
}
