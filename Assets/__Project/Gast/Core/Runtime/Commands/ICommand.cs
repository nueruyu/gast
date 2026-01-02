namespace DescrioGames.Core.Commands
{
    /// <summary>
    /// Marker interface for a command that does not return a value.
    /// </summary>
    public interface ICommand
    {
    }

    /// <summary>
    /// Marker interface for a command that returns a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    public interface ICommand<TResult>
    {
    }
}