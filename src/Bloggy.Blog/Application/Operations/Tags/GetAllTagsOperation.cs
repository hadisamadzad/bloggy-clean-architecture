using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Models.Tags;
using Bloggy.Core.Utilities.OperationResult;

namespace Bloggy.Blog.Application.Operations.Tags;

public class GetAllTagsOperation(IRepositoryManager repository) :
    IOperation<GetAllTagsCommand, List<TagModel>>
{
    public async Task<OperationResult<List<TagModel>>> ExecuteAsync(GetAllTagsCommand command,
        CancellationToken? cancellation = null)
    {
        var tags = await repository.Tags.GetAllAsync();

        return OperationResult<List<TagModel>>.Success([.. tags.MapToModels()]);
    }
}

public record GetAllTagsCommand : IOperationCommand;
