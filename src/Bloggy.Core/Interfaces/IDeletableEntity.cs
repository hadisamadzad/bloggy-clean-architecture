namespace Bloggy.Core.Interfaces;

public interface IDeletableEntity : IEntity
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}