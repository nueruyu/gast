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
        /// Dynamically dispatches a deserialized command object.
        /// </summary>
        /// <param name="command">The command object, typically from a serializer.</param>
        /// <returns>The result from the command handler, or null if there is no return value.</returns>
        object Dispatch(object command);

        /// <summary>
        /// Statically dispatches an asynchronous command that does not return a value.
        /// </summary>
        ValueTask DispatchAsync<TCommand>(TCommand command) where TCommand : struct, IAsyncCommand;

        /// <summary>
        /// Statically dispatches an asynchronous command that returns a value.
        /// </summary>
        ValueTask<TResult> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : struct, IAsyncCommand<TResult>;

        /// <summary>
        /// Dynamically dispatches a deserialized asynchronous command object.
        /// </summary>
        /// <param name="command">The command object, typically from a serializer.</param>
        /// <returns>A task that completes with the result from the command handler, or completes with no result.</returns>
        ValueTask<object> DispatchAsync(object command);
    }
}
