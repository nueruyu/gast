using System.Threading;
using System.Threading.Tasks;

namespace Gast.Core.Commands
{
    /// <summary>
    /// Defines a handler for an asynchronous command that does not return a value.
    /// </summary>
    public interface IAsyncCommandHandler<TCommand>
        where TCommand : struct, IAsyncCommand
    {
        ValueTask ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Defines a handler for an asynchronous command that returns a value.
    /// </summary>
    public interface IAsyncCommandHandler<TCommand, TResult>
        where TCommand : struct, IAsyncCommand<TResult>
    {
        ValueTask<TResult> ExecuteAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}