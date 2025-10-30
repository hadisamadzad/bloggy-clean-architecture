using Bloggy.Blog.Application.Interfaces;
using Bloggy.Core.Helpers;
using Bloggy.Core.Utilities.OperationResult;
using FluentValidation;

namespace Bloggy.Blog.Application.Operations.Views;

public class CountArticleViewOperation(
    IViewMemoryRepository cache
    ) : IOperation<CountArticleViewCommand, NoResult>
{
    public async Task<OperationResult<NoResult>> ExecuteAsync(
        CountArticleViewCommand command, CancellationToken? cancellation = null)
    {
        var validation = new CountArticleViewValidator().Validate(command);
        if (!validation.IsValid)
            return OperationResult.ValidationFailure([.. validation.GetErrorMessages()]);

        // Add to Redis buffer. If visitor was newly added, mark pending; actual DB increment
        // will be performed by the flusher hosted service.
        await cache.AddUniqueViewAsync(command.ArticleId, command.VisitorId);

        // We always return success; actual increment is handled by the flusher.
        return OperationResult.Success();
    }
}

public record CountArticleViewCommand(string ArticleId, string VisitorId) : IOperationCommand;

public class CountArticleViewValidator : AbstractValidator<CountArticleViewCommand>
{
    public CountArticleViewValidator()
    {
        RuleFor(x => x.ArticleId).NotEmpty();
        RuleFor(x => x.VisitorId).NotEmpty().MaximumLength(200);
    }
}
