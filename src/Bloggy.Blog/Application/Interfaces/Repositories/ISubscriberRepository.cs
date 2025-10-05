using Bloggy.Blog.Application.Types.Entities;
using Bloggy.Core.Interfaces;

namespace Bloggy.Blog.Application.Interfaces.Repositories;

public interface ISubscriberRepository : IRepository<SubscriberEntity>
{
    Task<SubscriberEntity> GetByEmailAsync(string email);
}