using Bloggy.Identity.Application.Interfaces;
using Bloggy.Identity.Application.Interfaces.Repositories;
using Bloggy.Identity.Infrastructure.Database.Repositories;
using MongoDB.Driver;

namespace Bloggy.Identity.Infrastructure.Database;

public class RepositoryManager(IMongoDatabase mongoDatabase) : IRepositoryManager
{
    public IUserRepository Users { get; } =
        new UserRepository(mongoDatabase, "identity.users");
}
