using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.Selectors
{
    public class UtilitySelector<T> : IMethodSelector<T> where T : struct
    {
        public async UniTask<(Method<T>, T)> SelectAsync(
            IReadOnlyList<Method<T>> methods,
            T state,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            Method<T> bestMethod = null;
            var bestScore = float.NegativeInfinity;
            var bestState = state;

            foreach (var method in methods)
            {
                var (valid, resultState) = await ValidateMethod(method, state, options, cancellationToken);

                if (!valid)
                {
                    continue;
                }

                var score = method.GetScore(resultState);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMethod = method;
                    bestState = resultState;
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
            var (preferredMethod, resultState) = await SelectAsync(
                methods,
                state,
                options,
                cancellationToken);

            if (preferredMethod != null &&
                preferredMethod.GetScore(resultState) < currentMethod.GetScore(resultState))
                return preferredMethod;

            return null;
        }

        async UniTask<(bool, T)> ValidateMethod(
            Method<T> method,
            T state,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            if (!method.CheckCondition(state))
                return (false, state);

            if (options.MaxDepth != 0)
            {
                var nextOptions = options.StepDown();

                foreach (var task in method.SubTasks)
                {
                    var (valid, resultState) = await task.ValidateAsync(state, nextOptions, cancellationToken);

                    if (!valid)
                    {
                        return (false, state);
                    }

                    state = resultState;
                }
            }

            return (true, state);
        }
    }
}