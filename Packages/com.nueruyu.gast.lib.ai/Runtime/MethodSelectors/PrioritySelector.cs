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
                worldState.WriteTo(ref simulationState);
                var validationContext = new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

                if (await ValidateMethod(method, validationContext, cancellationToken))
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
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            var preferredMethod = await SelectAsync(
                methods,
                context,
                cancellationToken);

            if (preferredMethod != null &&
                preferredMethod.Index < currentMethod.Index)
                return preferredMethod;

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