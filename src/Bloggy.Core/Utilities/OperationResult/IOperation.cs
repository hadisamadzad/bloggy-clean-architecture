namespace Bloggy.Core.Utilities.OperationResult;

public interface IOperation<TCommand, TResult> where TCommand : IOperationCommand
{
    Task<OperationResult<TResult>> ExecuteAsync(TCommand command, CancellationToken cancellation);
}