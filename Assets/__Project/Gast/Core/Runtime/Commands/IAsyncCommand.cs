using System.Threading.Tasks;

namespace Gast.Core.Commands
{
    /// <summary>
    /// Marker interface for an asynchronous command that does not return a value.
    /// </summary>
    public interface IAsyncCommand
    {
    }

    /// <summary>
    /// Marker interface for an asynchronous command that returns a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    public interface IAsyncCommand<TResult>
    {
    }
}
