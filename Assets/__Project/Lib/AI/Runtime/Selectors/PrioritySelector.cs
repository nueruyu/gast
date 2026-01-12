using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.Selectors
{
    public class PrioritySelector<T> : IMethodSelector<T> where T : struct
    {
        public async UniTask<(Method<T>, T)> SelectAsync(
            IReadOnlyList<Method<T>> methods,
            T state,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            foreach (var method in methods)
            {
                var (valid, resultState) = await ValidateMethod(method, state, options, cancellationToken);

                if (valid)
                {
                    return (method, resultState);
                }
            }

            return (null, state);
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