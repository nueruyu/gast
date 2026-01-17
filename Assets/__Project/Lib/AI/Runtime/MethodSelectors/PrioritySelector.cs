using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Gast.Lib.AI.MethodSelectors
{
    public class PrioritySelector<TWorldState, TContext> : IMethodSelector<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly TWorldState simulationState = new();

        public async UniTask<Method<TWorldState, TContext>> SelectAsync(
            IReadOnlyList<Method<TWorldState, TContext>> methods,
            TWorldState worldState,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            foreach (var method in methods)
            {
                simulationState.CopyFrom(worldState);

                if (await ValidateMethod(method, simulationState, options, cancellationToken))
                {
                    worldState.CopyFrom(simulationState);
                    return method;
                }
            }

            return null;
        }

        public async UniTask<Method<TWorldState, TContext>> SelectInterruptsAsync(
            IReadOnlyList<Method<TWorldState, TContext>> methods,
            Method<TWorldState, TContext> currentMethod,
            TWorldState worldState,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            var preferredMethod = await SelectAsync(
                methods,
                worldState,
                options,
                cancellationToken);

            if (preferredMethod != null &&
                preferredMethod.Index < currentMethod.Index)
                return preferredMethod;

            return null;
        }

        async UniTask<bool> ValidateMethod(
           Method<TWorldState, TContext> method,
           TWorldState worldState,
           CheckOptions options,
           CancellationToken cancellationToken)
        {
            if (!method.CheckCondition(worldState))
                return false;

            if (options.MaxDepth != 0)
            {
                var nextOptions = options.StepDown();
                foreach (var task in method.SubTasks)
                {
                    if (!await task.ValidateAsync(worldState, nextOptions, cancellationToken))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}