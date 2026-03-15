using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI
{
    public interface IMethodSelector<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        UniTask<Method<TActorContext, TWorldState>> SelectAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken);

        UniTask<Method<TActorContext, TWorldState>> SelectInterruptsAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            CurrentMethodInfo<TActorContext, TWorldState> currentMethodInfo,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken);
    }
}
