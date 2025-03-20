using Blog.Application.Constants;
using Blog.Application.Interfaces;
using Common.Utilities.OperationResult;
using MediatR;

namespace Blog.Application.UseCases.Settings;

// Handler
public class GetBlogSettingsHandler(IRepositoryManager repository) :
    IRequestHandler<GetBlogSettingsQuery, OperationResult>
{
    public async Task<OperationResult> Handle(GetBlogSettingsQuery request, CancellationToken cancellationToken)
    {
        // Retrieve the article
        var entity = await repository.Settings.GetBlogSettingAsync();
        if (entity is null)
            return OperationResult.Failure(OperationStatus.Unprocessable, Errors.SettingsNotFound);

        return OperationResult.Success(entity);
    }
}

// Model
public record GetBlogSettingsQuery() : IRequest<OperationResult>;