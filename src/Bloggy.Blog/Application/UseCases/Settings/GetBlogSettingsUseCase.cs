using Bloggy.Blog.Application.Constants;
using Bloggy.Blog.Application.Interfaces;
using Bloggy.Core.Utilities.OperationResult;
using MediatR;

namespace Bloggy.Blog.Application.UseCases.Settings;

// Handler
public class GetBlogSettingsHandler(IRepositoryManager repository) :
    IRequestHandler<GetBlogSettingsQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetBlogSettingsQuery request, CancellationToken cancellationToken)
    {
        // Retrieve the article
        var entity = await repository.Settings.GetBlogSettingAsync();
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Failed, Errors.SettingsNotFound);

        return OperationResult.Success(entity);
    }
}

// Model
public record GetBlogSettingsQuery() : IRequest<OperationResult>;