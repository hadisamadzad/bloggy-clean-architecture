using Bloggy.Core.Interfaces;
using Bloggy.Identity.Application.Types.Entities;

namespace Bloggy.Identity.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<bool> AnyAsync();
    Task<UserEntity> GetByIdAsync(string id);
    Task<UserEntity> GetByEmailAsync(string email);
}
