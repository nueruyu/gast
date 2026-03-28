using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI.MethodSelectors
{
    public class PrioritySelector<TActorContext, TWorldState> : IMethodSelector<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        TWorldState simulationState;

        public async UniTask<Method<TActorContext, TWorldState>> SelectAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            var worldState = context.WorldState;

            foreach (var method in methods)
            {
                if (!method.CheckStartCondition(worldState))
                    continue;

                worldState.WriteTo(ref simulationState);
                var validationContext = new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

                if (await ValidateMethod(method, 0, validationContext, cancellationToken))
                {
                    simulationState.WriteTo(ref worldState);
                    return method;
                }
            }

            return null;
        }

        public async UniTask<Method<TActorContext, TWorldState>> SelectInterruptsAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            CurrentMethodInfo<TActorContext, TWorldState> currentMethodInfo,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            // Check if the current method is still valid from its current execution point.
            context.WorldState.WriteTo(ref simulationState);
            var validationContext = new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore, context.CurrentlyExecutingTask);

            var isCurrentMethodStillValid = await ValidateMethod(
                currentMethodInfo.Method,
                currentMethodInfo.NextSubTaskIndex,
                validationContext,
                cancellationToken);

            // Find the best method from the current state.
            var preferredMethod = await SelectAsync(
                methods,
                context,
                cancellationToken);

            // Evaluate interrupt conditions.
            if (!isCurrentMethodStillValid)
                return preferredMethod;

            if (preferredMethod != null &&
                preferredMethod.Index < currentMethodInfo.Method.Index)
                // Interrupt if a higher-priority method becomes available.
                return preferredMethod;

            return null;
        }

        async UniTask<bool> ValidateMethod(
            Method<TActorContext, TWorldState> method,
            int startIndex,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
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