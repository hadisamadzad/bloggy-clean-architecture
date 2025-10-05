using Bloggy.Blog.Application.Interfaces;
using Bloggy.Blog.Application.Types.Models.Settings;
using Bloggy.Core.Utilities.OperationResult;

namespace Bloggy.Blog.Application.Operations.Settings;

public class GetBlogSettingsOperation(IRepositoryManager repository) :
    IOperation<GetBlogSettingsCommand, SettingModel>
{
    public async Task<OperationResult<SettingModel>> ExecuteAsync(
        GetBlogSettingsCommand command, CancellationToken? cancellation = null)
    {
        // Retrieve the article
        var entity = await repository.Settings.GetBlogSettingAsync();
        if (entity is null)
            return OperationResult<SettingModel>.NotFoundFailure("Blog settings not found.");

        return OperationResult<SettingModel>.Success(entity.MapToModel());
    }
}

public record GetBlogSettingsCommand() : IOperationCommand;