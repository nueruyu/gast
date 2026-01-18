using System.Threading;

namespace Gast.Lib.AI
{
    public interface IContext<TContext, out TWorldState>
        where TContext : struct, IContext<TContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>, new()
    {
        object ActorId { get; }
        TWorldState WorldState { get; }
        CancellationToken CancellationToken { get; }

        TContext WithCancellationToken(CancellationToken cancellationToken);
    }
}
