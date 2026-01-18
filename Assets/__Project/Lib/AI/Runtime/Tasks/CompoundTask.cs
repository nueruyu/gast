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
        readonly TWorldState simulationState = new();

        internal CompoundTask(
            string name,
            IEnumerable<Method<TWorldState, TContext>> methods,
            IMethodSelector<TWorldState, TContext> methodSelector)
        {
            Name = name;
            this.methods = methods.ToArray();
            this.methodSelector = methodSelector;
        }

        public async UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            var method = await SelectCurrentMethodAsync(
                worldState,
                cancellationToken);

            return method != null;
        }

        public async UniTask RunAsync(TContext ctx)
        {
            var actorId = ctx.ActorId;
            if (actorId == null)
            {
                await UniTask.CompletedTask;
                return;
            }

            DebugLogger.EnterTask(actorId, Name);

            try
            {
                simulationState.CopyFrom(ctx.WorldState);

                var method = await SelectCurrentMethodAsync(
                    simulationState,
                    ctx.CancellationToken);

                if (method == null)
                {
                    DebugLogger.LogPlanFailed(actorId, $"No valid method for {Name}");
                    return;
                }

                DebugLogger.LogMethodSelected(actorId, Name, method.Name, ctx.WorldState);
                DebugLogger.LogPlan(actorId, method.SubTasks.Cast<ITask>().ToList());

                using var localCts = CancellationTokenSource.CreateLinkedTokenSource(ctx.CancellationToken);
                var localCtx = ctx.WithCancellationToken(localCts.Token);

                try
                {
                    await UniTask.WhenAny(
                        RunMethodAsync(method, localCtx),
                        MonitorInterruptsAsync(method, localCtx)
                    );
                }
                finally
                {
                    localCts.Cancel();
                }
            }
            finally
            {
                DebugLogger.ExitTask(actorId);
            }
        }

        async UniTask RunMethodAsync(
            Method<TWorldState, TContext> method,
            TContext ctx)
        {
            foreach (var task in method.SubTasks)
            {
                await task.RunAsync(ctx);
            }
        }

        async UniTask MonitorInterruptsAsync(
            Method<TWorldState, TContext> currentMethod,
            TContext ctx)
        {
            var actorId = ctx.ActorId;

            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, ctx.CancellationToken);

                simulationState.CopyFrom(ctx.WorldState);

                var interruptsMethod = await methodSelector.SelectInterruptsAsync(
                    methods,
                    currentMethod,
                    simulationState,
                    ctx.CancellationToken);

                if (interruptsMethod != null)
                {
                    DebugLogger.LogPlanFailed(actorId, $"Interrupt: {Name} switching to {interruptsMethod.Name}");
                    break;
                }
            }
        }

        UniTask<Method<TWorldState, TContext>> SelectCurrentMethodAsync(
           TWorldState worldState,
           CancellationToken cancellationToken)
        {
            return methodSelector.SelectAsync(methods, worldState, cancellationToken);
        }
    }
}
