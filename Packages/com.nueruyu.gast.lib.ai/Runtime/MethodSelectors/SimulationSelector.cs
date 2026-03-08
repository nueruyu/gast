using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.MethodSelectors
{
    public class SimulationSelector<TActorContext, TWorldState> : IMethodSelector<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly Func<TWorldState, float> worldEvaluator;
        readonly IEnvironmentModel<TWorldState> envModel;
        TWorldState simulationState;

        public SimulationSelector(Func<TWorldState, float> worldEvaluator, IEnvironmentModel<TWorldState> envModel = null)
        {
            this.worldEvaluator = worldEvaluator;
            this.envModel = envModel;
        }

        public async UniTask<Method<TActorContext, TWorldState>> SelectAsync(
            IReadOnlyList<Method<TActorContext, TWorldState>> methods,
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            Method<TActorContext, TWorldState> bestMethod = null;
            var bestOutcomeScore = float.NegativeInfinity;

            foreach (var method in methods)
            {
                if (!method.CheckCondition(worldState))
                    continue;

                worldState.WriteTo(ref simulationState);

                var valid = await SimulateMethodAsync(
                    method,
                    simulationState,
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
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            worldState.WriteTo(ref simulationState);

            await SimulateMethodAsync(
                currentMethod,
                simulationState,
                cancellationToken);

            var currentScore = worldEvaluator(simulationState);

            Method<TActorContext, TWorldState> bestMethod = null;
            var bestOutcomeScore = currentScore;

            foreach (var method in methods)
            {
                if (method == currentMethod)
                    continue;

                if (!method.CheckCondition(worldState))
                    continue;

                worldState.WriteTo(ref simulationState);

                var valid = await SimulateMethodAsync(
                    method,
                    simulationState,
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
           TWorldState worldState,
           CancellationToken cancellationToken)
        {
            foreach (var subTask in method.SubTasks)
            {
                var valid = await subTask.ValidateAsync(worldState, cancellationToken);
                if (!valid)
                {
                    return false;
                }

                envModel?.Simulate(worldState);
            }

            return true;
        }
    }

    public interface IEnvironmentModel<TWorldState> where TWorldState : class
    {
        void Simulate(TWorldState state);
    }
}
