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

                if (await ValidateMethod(method, 0, validationContext, cancellationToken))
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
            CurrentMethodInfo<TActorContext, TWorldState> currentMethodInfo,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            context.WorldState.WriteTo(ref simulationState);
            var validationContext = new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore, context.CurrentlyExecutingTask);

            var isCurrentMethodStillValid = await ValidateMethod(
                currentMethodInfo.Method,
                currentMethodInfo.NextSubTaskIndex,
                validationContext,
                cancellationToken);

            var preferredMethod = await SelectAsync(
                methods,
                context,
                cancellationToken);

            if (!isCurrentMethodStillValid)
                return preferredMethod;

            if (preferredMethod == null || preferredMethod == currentMethodInfo.Method)
                return null;

            var currentScore = currentMethodInfo.Method.GetScore(context.WorldState);
            var interruptionCost = currentMethodInfo.Method.GetInterruptionCost(context.WorldState);
            var newScore = preferredMethod.GetScore(context.WorldState);

            if (newScore > currentScore + interruptionCost)
                return preferredMethod;

            return null;
        }

        async UniTask<bool> ValidateMethod(
            Method<TActorContext, TWorldState> method,
            int startIndex,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            if (startIndex == 0)
            {
                if (!method.CheckStartCondition(context.WorldState))
                    return false;
            }

            for (var i = startIndex; i < method.SubTasks.Count; i++)
            {
                if (!method.CheckContinuationCondition(context.WorldState))
                    return false;

                var task = method.SubTasks[i];
                if (!await task.ValidateAsync(context, cancellationToken))
                    return false;
            }

            return true;
        }
    }
}