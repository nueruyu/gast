using DescrioGames.Core.Commands;

namespace DescrioGames.Api
{
    /// <summary>
    /// Defines a handler for a command that does not return a value.
    /// </summary>
    public interface ICommandHandler<TCommand>
        where TCommand : struct, ICommand
    {
        void Execute(in TCommand command);
    }

    /// <summary>
    /// Defines a handler for a command that returns a value.
    /// </summary>
    public interface ICommandHandler<TCommand, TResult>
        where TCommand : struct, ICommand<TResult>
    {
        TResult Execute(in TCommand command);
    }
}