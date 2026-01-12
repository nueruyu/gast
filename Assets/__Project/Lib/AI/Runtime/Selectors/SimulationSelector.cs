using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.Selectors
{
    public class SimulationSelector<T> : IMethodSelector<T> where T : struct
    {
        readonly Func<T, float> worldEvaluator;
        readonly IEnvironmentModel<T> envModel;

        public SimulationSelector(Func<T, float> worldEvaluator, IEnvironmentModel<T> envModel = null)
        {
            this.worldEvaluator = worldEvaluator;
            this.envModel = envModel;
        }

        public async UniTask<(Method<T>, T)> SelectAsync(
            IReadOnlyList<Method<T>> methods,
            T state,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            Method<T> bestMethod = null;
            var bestOutcomeScore = float.NegativeInfinity;
            var bestState = state;

            foreach (var method in methods)
            {
                if (!method.CheckCondition(state))
                    continue;

                var (valid, resultState) = await SimulateMethodAsync(
                    method,
                    state,
                    options,
                    cancellationToken);

                if (valid)
                {
                    var outcomeScore = worldEvaluator(resultState);
                    if (outcomeScore > bestOutcomeScore)
                    {
                        bestOutcomeScore = outcomeScore;
                        bestMethod = method;
                        bestState = resultState;
                    }
                }
            }

            if (bestMethod == null)
            {
                return (null, state);
            }

            return (bestMethod, bestState);
        }

        public async UniTask<Method<T>> SelectInterruptsAsync(
            IReadOnlyList<Method<T>> methods,
            Method<T> currentMethod,
            T state,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            var (currentMethodValid, currentResultState) = await SimulateMethodAsync(
                currentMethod,
                state,
                options,
                cancellationToken);
            var currentScore = worldEvaluator(currentResultState);

            Method<T> bestMethod = null;
            var bestOutcomeScore = currentScore;

            foreach (var method in methods)
            {
                if (method == currentMethod)
                    continue;

                if (!method.CheckCondition(state))
                    continue;

                var (valid, resultState) = await SimulateMethodAsync(
                    method,
                    state,
                    options,
                    cancellationToken);

                if (valid)
                {
                    var outcomeScore = worldEvaluator(resultState);
                    if (outcomeScore > bestOutcomeScore)
                    {
                        bestOutcomeScore = outcomeScore;
                        bestMethod = method;
                    }
                }
            }

            if (bestMethod == null)
            {
                return null;
            }

            return bestMethod;
        }

        async UniTask<(bool, T)> SimulateMethodAsync(
            Method<T> method,
            T state,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            var nextOptions = options.StepDown();
            foreach (var subTask in method.SubTasks)
            {
                var (valid, resultState) = await subTask.ValidateAsync(state, nextOptions, cancellationToken);

                if (!valid)
                {
                    return (false, resultState);
                }

                state = resultState;
                envModel?.Simulate(ref state);
            }

            return (true, state);
        }
    }

    public interface IEnvironmentModel<TWorldState> where TWorldState : struct
    {
        void Simulate(ref TWorldState state);
    }
}