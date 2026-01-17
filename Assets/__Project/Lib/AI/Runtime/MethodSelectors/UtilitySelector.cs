using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.MethodSelectors
{
    public class UtilitySelector<TWorldState, TContext> : IMethodSelector<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly TWorldState simulationState = new();

        public async UniTask<Method<TWorldState, TContext>> SelectAsync(
            IReadOnlyList<Method<TWorldState, TContext>> methods,
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            Method<TWorldState, TContext> bestMethod = null;
            var bestScore = float.NegativeInfinity;

            foreach (var method in methods)
            {
                simulationState.CopyFrom(worldState);

                if (!await ValidateMethod(method, simulationState, cancellationToken))
                {
                    continue;
                }

                var score = method.GetScore(worldState);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMethod = method;
                    worldState.CopyFrom(simulationState);
                }
            }

            return bestMethod;
        }

        public async UniTask<Method<TWorldState, TContext>> SelectInterruptsAsync(
            IReadOnlyList<Method<TWorldState, TContext>> methods,
            Method<TWorldState, TContext> currentMethod,
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            var preferredMethod = await SelectAsync(
                methods,
                worldState,
                cancellationToken);

            if (preferredMethod != null &&
                preferredMethod.GetScore(worldState) < currentMethod.GetScore(worldState))
                return preferredMethod;

            return null;
        }

        async UniTask<bool> ValidateMethod(
           Method<TWorldState, TContext> method,
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