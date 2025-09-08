using Blog.Application.Interfaces.Repositories;
using Blog.Application.Types.Entities;
using Common.Persistence.MongoDB;
using MongoDB.Driver;

namespace Blog.Infrastructure.Database.Repositories;

public class TagRepository(IMongoDatabase database, string collectionName) :
    MongoDbRepositoryBase<TagEntity>(database, collectionName), ITagRepository
{
    public async Task<TagEntity> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.Id == id).SingleOrDefaultAsync();
    }

    public async Task<List<TagEntity>> GetByIdsAsync(IEnumerable<string> ids)
    {
        return await _collection.Find(x => ids.Contains(x.Id)).ToListAsync();
    }

    public async Task<TagEntity> GetBySlugAsync(string slug)
    {
        return await _collection.Find(x => x.Slug == slug).SingleOrDefaultAsync();
    }
}
