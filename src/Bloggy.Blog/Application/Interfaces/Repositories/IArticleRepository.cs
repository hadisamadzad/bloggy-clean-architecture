using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Blog.Application.Types.Models.Articles;
using Bloggy.Core.Interfaces;

namespace Bloggy.Blog.Application.Interfaces.Repositories;

public interface IArticleRepository : IRepository<ArticleEntity>
{
    Task<ArticleEntity> GetByIdAsync(string id);
    Task<ArticleEntity> GetBySlugAsync(string slug);
    Task<List<ArticleEntity>> GetByIdsAsync(IEnumerable<string> ids);
    Task<List<ArticleEntity>> GetByFilterAsync(ArticleFilter filter);
    Task<int> CountByFilterAsync(ArticleFilter filter);
}