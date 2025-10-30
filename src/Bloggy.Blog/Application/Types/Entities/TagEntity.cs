using Bloggy.Core.Interfaces;

namespace Bloggy.Blog.Application.Types.Entities;

public class TagEntity : IEntity
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}