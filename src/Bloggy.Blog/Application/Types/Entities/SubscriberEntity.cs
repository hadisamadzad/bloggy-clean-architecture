using Bloggy.Core.Interfaces;

namespace Bloggy.Blog.Application.Types.Entities;

public class SubscriberEntity : IEntity
{
    public required string Id { get; set; }
    public required string Email { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}