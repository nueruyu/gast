using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI.MethodSelectors
{
    public class UtilitySelector<TActorContext, TWorldState> : IMethodSelector<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        TWorldState simulationState;

        public async UniTask<Method<TActorContext, TWorldState>> SelectAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            Method<TActorContext, TWorldState> bestMethod = null;
            var bestScore = float.MinValue;

            foreach (var method in methods)
            {
                context.WorldState.WriteTo(ref simulationState);
                var validationContext = new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

                if (await ValidateMethod(method, validationContext, cancellationToken))
                {
                    var score = method.GetScore(context.WorldState);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMethod = method;
                    }
                }
            }

            return bestMethod;
        }

        public async UniTask<Method<TActorContext, TWorldState>> SelectInterruptsAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            Method<TActorContext, TWorldState> currentMethod,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            var preferredMethod = await SelectAsync(
                methods,
                context,
                cancellationToken);

            if (preferredMethod == null || preferredMethod == currentMethod)
                return null;

            var currentScore = currentMethod.GetScore(context.WorldState);
            var interruptionCost = currentMethod.GetInterruptionCost(context.WorldState);
            var newScore = preferredMethod.GetScore(context.WorldState);

            if (newScore > currentScore + interruptionCost) return preferredMethod;

            return null;
        }

        async UniTask<bool> ValidateMethod(
            Method<TActorContext, TWorldState> method,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            if (!method.CheckCondition(context.WorldState))
                return false;

            foreach (var task in method.SubTasks)
                if (!await task.ValidateAsync(context, cancellationToken))
                    return false;

            return true;
        }
    }
}