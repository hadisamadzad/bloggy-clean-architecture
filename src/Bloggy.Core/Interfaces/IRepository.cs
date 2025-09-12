using System.Linq.Expressions;

namespace Bloggy.Core.Interfaces;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    // Queries
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> GetByExpressionAsync(Expression<Func<TEntity, bool>> predicate);

    // Commands
    Task InsertAsync(TEntity entity);
    Task InsertAsync(IEnumerable<TEntity> entities);

    Task<bool> UpdateAsync(TEntity entity);
    Task<bool> UpdateAsync(IEnumerable<TEntity> entities);

    Task<bool> DeleteAsync(TEntity entity);
    Task<bool> DeleteAsync(IEnumerable<TEntity> entities);
}