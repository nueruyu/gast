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
            Method<TActorContext, TWorldState> currentMethod,
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            context.WorldState.WriteTo(ref simulationState);
            var validationContext = new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

            await SimulateMethodAsync(
                currentMethod,
                validationContext,
                cancellationToken);

            var currentScore = worldEvaluator(simulationState);

            Method<TActorContext, TWorldState> bestMethod = null;
            var bestOutcomeScore = currentScore;

            var worldState = context.WorldState;

            foreach (var method in methods)
            {
                if (method == currentMethod)
                    continue;

                if (!method.CheckCondition(worldState))
                    continue;

                worldState.WriteTo(ref simulationState);
                var innerValidationContext =
                    new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

                var valid = await SimulateMethodAsync(
                    method,
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
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            foreach (var subTask in method.SubTasks)
            {
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