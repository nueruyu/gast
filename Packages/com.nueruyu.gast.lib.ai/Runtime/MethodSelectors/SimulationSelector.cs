using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI.MethodSelectors
{
    public class SimulationSelector<TActorContext, TWorldState> : IMethodSelector<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly IEnvironmentModel<TWorldState> envModel;
        readonly Func<TWorldState, float> worldEvaluator;
        TWorldState simulationState;

        public SimulationSelector(Func<TWorldState, float> worldEvaluator,
            IEnvironmentModel<TWorldState> envModel = null)
        {
            this.worldEvaluator = worldEvaluator;
            this.envModel = envModel;
        }

        public async UniTask<Method<TActorContext, TWorldState>> SelectAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            Method<TActorContext, TWorldState> bestMethod = null;
            var bestOutcomeScore = float.NegativeInfinity;

            var worldState = context.WorldState;

            foreach (var method in methods)
            {
                if (!method.CheckCondition(worldState))
                    continue;

                worldState.WriteTo(ref simulationState);
                var validationContext = new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

                var valid = await SimulateMethodAsync(
                    method,
                    0,
                    validationContext,
                    cancellationToken);

                if (valid)
                {
                    var outcomeScore = worldEvaluator(simulationState);
                    if (outcomeScore > bestOutcomeScore)
                    {
                        bestOutcomeScore = outcomeScore;
                        bestMethod = method;
                        simulationState.WriteTo(ref worldState);
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
            var currentMethodValidationContext =
                new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

            var isCurrentMethodStillValid = await SimulateMethodAsync(
                currentMethodInfo.Method,
                currentMethodInfo.NextSubTaskIndex,
                currentMethodValidationContext,
                cancellationToken);

            if (!isCurrentMethodStillValid)
                return await SelectAsync(methods, context, cancellationToken);

            var currentScore = worldEvaluator(simulationState);

            Method<TActorContext, TWorldState> bestMethod = null;
            var bestOutcomeScore = currentScore;

            var worldState = context.WorldState;

            foreach (var method in methods)
            {
                if (method == currentMethodInfo.Method)
                    continue;

                if (!method.CheckCondition(worldState))
                    continue;

                worldState.WriteTo(ref simulationState);
                var innerValidationContext =
                    new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

                var valid = await SimulateMethodAsync(
                    method,
                    0,
                    innerValidationContext,
                    cancellationToken);

                if (valid)
                {
                    var outcomeScore = worldEvaluator(simulationState);
                    if (outcomeScore > bestOutcomeScore)
                    {
                        bestOutcomeScore = outcomeScore;
                        bestMethod = method;
                        simulationState.WriteTo(ref worldState);
                    }
                }
            }

            return bestMethod;
        }

        async UniTask<bool> SimulateMethodAsync(
            Method<TActorContext, TWorldState> method,
            int startIndex,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            if (!method.CheckCondition(context.WorldState))
                return false;

            for (var i = startIndex; i < method.SubTasks.Count; i++)
            {
                var subTask = method.SubTasks[i];
                var valid = await subTask.ValidateAsync(context, cancellationToken);
                if (!valid) return false;

                envModel?.Simulate(context.WorldState);
            }

            return true;
        }
    }

    public interface IEnvironmentModel<TWorldState> where TWorldState : class
    {
        void Simulate(TWorldState state);
    }
}