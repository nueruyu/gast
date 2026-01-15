using System.Threading;

namespace Gast.Lib.AI
{
    /// <summary>
    /// Defines the context for AI task execution, providing access
    /// to the current world state and a cancellation token.
    /// </summary>
    public interface IContext<out TWorldState> where TWorldState : class, IWorldState<TWorldState>, new()
    {
        TWorldState WorldState { get; }
        CancellationToken CancellationToken { get; }
    }
}