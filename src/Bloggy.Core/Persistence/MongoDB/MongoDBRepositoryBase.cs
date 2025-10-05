using System.Linq.Expressions;
using Bloggy.Core.Interfaces;
using MongoDB.Driver;

namespace Bloggy.Core.Persistence.MongoDB;

public class MongoDbRepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    protected readonly IMongoCollection<TEntity> _collection;

    protected MongoDbRepositoryBase(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<TEntity>(collectionName);
    }

    #region Queries

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _collection.Find(predicate).AnyAsync();
    }

    public async Task<List<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _collection.Find(predicate).ToListAsync();
    }

    #endregion

    #region Commands

    public async Task InsertAsync(TEntity entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task InsertAsync(IEnumerable<TEntity> entities)
    {
        await _collection.InsertManyAsync(entities);
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        var options = new ReplaceOptions { IsUpsert = false };
        var result = await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, options);

        return result.IsAcknowledged && result.MatchedCount == 1;
    }

    public async Task<bool> UpdateAsync(IEnumerable<TEntity> entities)
    {
        var bulkOps = new List<WriteModel<TEntity>>();

        foreach (var entity in entities)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
            var replaceOne = new ReplaceOneModel<TEntity>(filter, entity);
            bulkOps.Add(replaceOne);
        }

        // Perform bulk write operation
        var result = await _collection.BulkWriteAsync(bulkOps);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == entity.Id);

        return result.IsAcknowledged && result.DeletedCount == 1;
    }

    public async Task<bool> DeleteAsync(IEnumerable<TEntity> entities)
    {
        var result = await _collection.DeleteManyAsync(
            x => entities.Select(x => x.Id).Distinct().Contains(x.Id));

        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    #endregion
}
