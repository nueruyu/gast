using System.Threading.Tasks;
using Gast.Core.Commands;

namespace Gast.Application
{
    /// <summary>
    /// Defines a handler for an asynchronous command that does not return a value.
    /// </summary>
    public interface IAsyncCommandHandler<TCommand>
        where TCommand : struct, IAsyncCommand
    {
        ValueTask ExecuteAsync(TCommand command);
    }

    /// <summary>
    /// Defines a handler for an asynchronous command that returns a value.
    /// </summary>
    public interface IAsyncCommandHandler<TCommand, TResult>
        where TCommand : struct, IAsyncCommand<TResult>
    {
        ValueTask<TResult> ExecuteAsync(TCommand command);
    }
}
