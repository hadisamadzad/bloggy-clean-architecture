using Blog.Infrastructure.Database.Extensions;
using Bloggy.Blog.Application.Interfaces.Repositories;
using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Core.Persistence.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Blog.Infrastructure.Database.Repositories;

public class ArticleRepository(IMongoDatabase database, string collectionName) :
    MongoDbRepositoryBase<ArticleEntity>(database, collectionName), IArticleRepository
{
    public async Task<ArticleEntity> GetByIdAsync(string id)
    {
        return await _collection.Find(x => x.Id == id).SingleOrDefaultAsync();
    }
    public async Task<ArticleEntity> GetBySlugAsync(string slug)
    {
        return await _collection.Find(x => x.Slug == slug).SingleOrDefaultAsync();
    }
    public async Task<List<ArticleEntity>> GetByIdsAsync(IEnumerable<string> ids)
    {
        return await _collection.Find(x => ids.Contains(x.Id)).ToListAsync();
    }

    public async Task<List<ArticleEntity>> GetByFilterAsync(ArticleFilter filter)
    {
        var query = _collection.AsQueryable()
            .Where(x => !x.IsDeleted);
        query = query.ApplyFilter(filter);
        query = query.ApplySort(filter.SortBy);

        if (filter.HasPagination)
            query = query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize);

        return await query.ToListAsync();
    }

    public async Task<int> CountByFilterAsync(ArticleFilter filter)
    {
        var query = _collection.AsQueryable()
            .Where(x => !x.IsDeleted);
        query = query.ApplyFilter(filter);
        return await query.CountAsync();
    }
}
