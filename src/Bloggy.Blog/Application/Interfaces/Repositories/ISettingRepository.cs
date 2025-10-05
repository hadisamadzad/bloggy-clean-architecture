using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Interfaces;

namespace Bloggy.Blog.Application.Interfaces.Repositories;

public interface ISettingRepository : IRepository<SettingEntity>
{
    Task<SettingEntity> GetBlogSettingAsync();
}