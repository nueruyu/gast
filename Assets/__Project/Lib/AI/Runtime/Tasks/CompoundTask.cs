using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Gast.Lib.AI.Tasks
{
    public class CompoundTask<TWorldState, TContext> : ITask<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        public string Name { get; }

        readonly Method<TWorldState, TContext>[] methods;
        readonly IMethodSelector<TWorldState, TContext> methodSelector;
        readonly int localDepthLimit;
        readonly TWorldState simulationState = new();

        internal CompoundTask(
            string name,
            IEnumerable<Method<TWorldState, TContext>> methods,
            IMethodSelector<TWorldState, TContext> methodSelector,
            int localDepthLimit)
        {
            Name = name;
            this.methods = methods.ToArray();
            this.methodSelector = methodSelector;
            this.localDepthLimit = localDepthLimit;
        }

        public async UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            var method = await SelectCurrentMethodAsync(
                worldState,
                options,
                cancellationToken);

            return method != null;
        }

        public async UniTask RunAsync(TContext ctx, CheckOptions? options)
        {
            var effectiveOptions = GetEffectiveOptions(options);

            simulationState.CopyFrom(ctx.WorldState);

            var method = await SelectCurrentMethodAsync(
                simulationState,
                effectiveOptions,
                ctx.CancellationToken);

            if (method == null)
            {
                DebugLogger.LogPlanFailed($"No valid method for {Name}");
                return;
            }

            DebugLogger.LogMethodSelected(Name, method.Name, ctx.WorldState);

            using var localCts = CancellationTokenSource.CreateLinkedTokenSource(ctx.CancellationToken);
            var localCtx = ctx.WithCancellationToken(localCts.Token);

            try
            {
                await UniTask.WhenAny(
                    RunMethodAsync(method, localCtx, effectiveOptions),
                    MonitorInterruptsAsync(method, localCtx, effectiveOptions)
                );
            }
            finally
            {
                localCts.Cancel();
            }
        }

        async UniTask RunMethodAsync(
            Method<TWorldState, TContext> method,
            TContext ctx,
            CheckOptions options)
        {
            var nextOptions = options.StepDown();
            foreach (var task in method.SubTasks)
            {
                await task.RunAsync(ctx, nextOptions);
            }
        }

        async UniTask MonitorInterruptsAsync(
            Method<TWorldState, TContext> currentMethod,
            TContext ctx,
            CheckOptions options)
        {
            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, ctx.CancellationToken);

                simulationState.CopyFrom(ctx.WorldState);

                var interruptsMethod = await methodSelector.SelectInterruptsAsync(
                    methods,
                    currentMethod,
                    simulationState,
                    options,
                    ctx.CancellationToken);

                if (interruptsMethod != null)
                {
                    DebugLogger.LogPlanFailed($"Interrupt: {Name} switching to {interruptsMethod.Name}");
                    break;
                }
            }
        }

        UniTask<Method<TWorldState, TContext>> SelectCurrentMethodAsync(
           TWorldState worldState,
           CheckOptions? options,
           CancellationToken cancellationToken)
        {
            var effectiveOptions = GetEffectiveOptions(options);
            return methodSelector.SelectAsync(methods, worldState, effectiveOptions, cancellationToken);
        }

        CheckOptions GetEffectiveOptions(CheckOptions? options)
        {
            return CheckOptions.Resolve(
                options ?? CheckOptions.Deep,
                localDepthLimit);
        }
    }
}