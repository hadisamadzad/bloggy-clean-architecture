using Bloggy.Identity.Application.Interfaces.Repositories;

namespace Bloggy.Identity.Application.Interfaces;

public interface IRepositoryManager
{
    IUserRepository Users { get; }

    Task<bool> CommitAsync();
}
