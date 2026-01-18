using System.Threading;
using System.Threading.Tasks;

namespace Gast.Core.Commands
{
    /// <summary>
    /// Dispatches commands to their respective handlers.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Statically dispatches a command that does not return a value. (Boxing-free)
        /// </summary>
        void Dispatch<TCommand>(in TCommand command) where TCommand : struct, ICommand;

        /// <summary>
        /// Statically dispatches a command that returns a value. (Boxing-free)
        /// </summary>
        TResult Dispatch<TCommand, TResult>(in TCommand command) where TCommand : struct, ICommand<TResult>;

        /// <summary>
        /// Statically dispatches an asynchronous command that does not return a value.
        /// </summary>
        ValueTask DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : struct, IAsyncCommand;

        /// <summary>
        /// Statically dispatches an asynchronous command that returns a value.
        /// </summary>
        ValueTask<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) where TCommand : struct, IAsyncCommand<TResult>;
    }
}