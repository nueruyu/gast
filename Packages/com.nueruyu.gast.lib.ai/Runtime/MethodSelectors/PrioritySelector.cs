using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.MethodSelectors
{
    public class PrioritySelector<TActorContext, TWorldState> : IMethodSelector<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        TWorldState simulationState;

        public async UniTask<Method<TActorContext, TWorldState>> SelectAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            foreach (var method in methods)
            {
                worldState.WriteTo(ref simulationState);

                if (await ValidateMethod(method, simulationState, cancellationToken))
                {
                    simulationState.WriteTo(ref worldState);
                    return method;
                }
            }

            return null;
        }

        public async UniTask<Method<TActorContext, TWorldState>> SelectInterruptsAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            Method<TActorContext, TWorldState> currentMethod,
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            var preferredMethod = await SelectAsync(
                methods,
                worldState,
                cancellationToken);

            if (preferredMethod != null &&
                preferredMethod.Index < currentMethod.Index)
                return preferredMethod;

            return null;
        }

        async UniTask<bool> ValidateMethod(
           Method<TActorContext, TWorldState> method,
           TWorldState worldState,
           CancellationToken cancellationToken)
        {
            if (!method.CheckCondition(worldState))
                return false;

            foreach (var task in method.SubTasks)
            {
                if (!await task.ValidateAsync(worldState, cancellationToken))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
