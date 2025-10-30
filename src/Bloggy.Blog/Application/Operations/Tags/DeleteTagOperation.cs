using Bloggy.Blog.Application.Interfaces;
using Bloggy.Core.Utilities.OperationResult;

namespace Bloggy.Blog.Application.Operations.Tags;

public class DeleteTagOperation(IRepositoryManager repository) :
    IOperation<DeleteTagCommand, NoResult>
{
    public async Task<OperationResult<NoResult>> ExecuteAsync(
        DeleteTagCommand command, CancellationToken? cancellation = null)
    {
        var entity = await repository.Tags.GetByIdAsync(command.TagId);
        if (entity is null)
            return OperationResult.NotFoundFailure("Tag not found.");

        // Soft delete: mark inactive
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;

        var updated = await repository.Tags.UpdateAsync(entity);
        if (!updated)
            return OperationResult.Failure("Failed to delete tag.");

        return OperationResult.Success();
    }
}

public record DeleteTagCommand(string TagId) : IOperationCommand;
