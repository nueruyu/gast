using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI
{
    public interface IMethodSelector<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TWorldState>
    {
        UniTask<Method<TWorldState, TContext>> SelectAsync(
            IReadOnlyList<Method<TWorldState, TContext>> methods,
            TWorldState worldState,
            CheckOptions options,
            CancellationToken cancellationToken);

        UniTask<Method<TWorldState, TContext>> SelectInterruptsAsync(
            IReadOnlyList<Method<TWorldState, TContext>> methods,
            Method<TWorldState, TContext> currentMethod,
            TWorldState worldState,
            CheckOptions options,
            CancellationToken cancellationToken);
    }
}
