using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.MethodSelectors
{
    public class SimulationSelector<TWorldState, TContext> : IMethodSelector<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly Func<TWorldState, float> worldEvaluator;
        readonly IEnvironmentModel<TWorldState> envModel;
        readonly TWorldState simulationState = new();

        public SimulationSelector(Func<TWorldState, float> worldEvaluator, IEnvironmentModel<TWorldState> envModel = null)
        {
            this.worldEvaluator = worldEvaluator;
            this.envModel = envModel;
        }

        public async UniTask<Method<TWorldState, TContext>> SelectAsync(
            IReadOnlyList<Method<TWorldState, TContext>> methods,
            TWorldState worldState,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            Method<TWorldState, TContext> bestMethod = null;
            var bestOutcomeScore = float.NegativeInfinity;

            foreach (var method in methods)
            {
                if (!method.CheckCondition(worldState))
                    continue;

                simulationState.CopyFrom(worldState);

                var valid = await SimulateMethodAsync(
                    method,
                    simulationState,
                    options,
                    cancellationToken);

                if (valid)
                {
                    var outcomeScore = worldEvaluator(simulationState);
                    if (outcomeScore > bestOutcomeScore)
                    {
                        bestOutcomeScore = outcomeScore;
                        bestMethod = method;
                        worldState.CopyFrom(simulationState);
                    }
                }
            }

            return bestMethod;
        }

        public async UniTask<Method<TWorldState, TContext>> SelectInterruptsAsync(
            IReadOnlyList<Method<TWorldState, TContext>> methods,
            Method<TWorldState, TContext> currentMethod,
            TWorldState worldState,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            simulationState.CopyFrom(worldState);

            await SimulateMethodAsync(
                currentMethod,
                simulationState,
                options,
                cancellationToken);

            var currentScore = worldEvaluator(simulationState);

            Method<TWorldState, TContext> bestMethod = null;
            var bestOutcomeScore = currentScore;

            foreach (var method in methods)
            {
                if (method == currentMethod)
                    continue;

                if (!method.CheckCondition(worldState))
                    continue;

                simulationState.CopyFrom(worldState);

                var valid = await SimulateMethodAsync(
                    method,
                    simulationState,
                    options,
                    cancellationToken);

                if (valid)
                {
                    var outcomeScore = worldEvaluator(simulationState);
                    if (outcomeScore > bestOutcomeScore)
                    {
                        bestOutcomeScore = outcomeScore;
                        bestMethod = method;
                        worldState.CopyFrom(simulationState);
                    }
                }
            }

            return bestMethod;
        }

        async UniTask<bool> SimulateMethodAsync(
           Method<TWorldState, TContext> method,
           TWorldState worldState,
           CheckOptions options,
           CancellationToken cancellationToken)
        {
            var nextOptions = options.StepDown();
            foreach (var subTask in method.SubTasks)
            {
                var valid = await subTask.ValidateAsync(worldState, nextOptions, cancellationToken);
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