using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI
{
    public interface IMethodSelector<TWorldState> where TWorldState : struct
    {
        UniTask<(Method<TWorldState>, TWorldState)> SelectAsync(
            IReadOnlyList<Method<TWorldState>> methods,
            TWorldState state,
            CheckOptions options,
            CancellationToken cancellationToken);

        UniTask<Method<TWorldState>> SelectInterruptsAsync(
            IReadOnlyList<Method<TWorldState>> methods,
            Method<TWorldState> currentMethod,
            TWorldState state,
            CheckOptions options,
            CancellationToken cancellationToken);
    }
}