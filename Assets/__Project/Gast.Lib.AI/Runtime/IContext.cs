using System.Threading;

namespace Gast.Lib.AI
{
    public interface IContext<TContext, out TWorldState>
        where TContext : struct, IContext<TContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>, new()
    {
        ContextKey ContextKey { get; }
        TWorldState WorldState { get; }
        CancellationToken CancellationToken { get; }

        void UpdateWorldState();

        TContext WithCancellationToken(CancellationToken cancellationToken);
    }
}