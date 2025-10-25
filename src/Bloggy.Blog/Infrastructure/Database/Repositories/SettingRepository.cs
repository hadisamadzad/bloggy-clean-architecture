using Bloggy.Blog.Application.Interfaces.Repositories;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Persistence.MongoDB;
using MongoDB.Driver;

namespace Bloggy.Blog.Infrastructure.Database.Repositories;

public class SettingRepository(IMongoDatabase database, string collectionName) :
    MongoDbRepositoryBase<SettingEntity>(database, collectionName), ISettingRepository
{
    public async Task<SettingEntity> GetBlogSettingAsync()
    {
        return await _collection.Find(x => x.Id == "blog_settings").SingleOrDefaultAsync();
    }
}
