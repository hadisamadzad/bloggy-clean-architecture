using Blog.Application.Types.Entities;
using Common.Interfaces;

namespace Blog.Application.Interfaces.Repositories;

public interface ITagRepository : IRepository<TagEntity>
{
    Task<TagEntity> GetByIdAsync(string id);
    Task<List<TagEntity>> GetByIdsAsync(IEnumerable<string> ids);
    Task<TagEntity> GetBySlugAsync(string slug);
}