using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Gast.Lib.AI.Tasks
{
    public class CompoundTask<TActorContext, TWorldState> : ITask<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TActorContext : class, IActorContext<TWorldState>
    {
        public string Name { get; }

        readonly Method<TActorContext, TWorldState>[] methods;
        readonly IMethodSelector<TActorContext, TWorldState> methodSelector;
        readonly TWorldState simulationState = new();

        internal CompoundTask(
            string name,
            IEnumerable<Method<TActorContext, TWorldState>> methods,
            IMethodSelector<TActorContext, TWorldState> methodSelector)
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

        public async UniTask RunAsync(AIContext<TActorContext> context, CancellationToken cancellationToken)
        {
            var contextKey = context.Key;
            var actorContext = context.ActorContext;

            DebugLogger.EnterTask(contextKey, Name);

            try
            {
                simulationState.CopyFrom(actorContext.WorldState);

                var method = await SelectCurrentMethodAsync(
                    simulationState,
                    cancellationToken);

                if (method == null)
                {
                    DebugLogger.LogPlanFailed(contextKey, $"No valid method for {Name}");
                    return;
                }

                DebugLogger.LogMethodSelected(contextKey, Name, method.Name, actorContext.WorldState);
                DebugLogger.LogPlan(contextKey, method.SubTasks.Cast<ITask>());

                using var localCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                try
                {
                    await UniTask.WhenAny(
                        RunMethodAsync(method, context, localCts.Token),
                        MonitorInterruptsAsync(method, context, localCts.Token)
                    );
                }
                finally
                {
                    localCts.Cancel();
                }
            }
            finally
            {
                DebugLogger.ExitTask(contextKey);
            }
        }

        async UniTask RunMethodAsync(
            Method<TActorContext, TWorldState> method,
            AIContext<TActorContext> context,
            CancellationToken cancellationToken)
        {
            foreach (var task in method.SubTasks)
            {
                await task.RunAsync(context, cancellationToken);

                context.ActorContext.UpdateWorldState();
            }
        }

        async UniTask MonitorInterruptsAsync(
            Method<TActorContext, TWorldState> currentMethod,
            AIContext<TActorContext> context,
            CancellationToken cancellationToken)
        {
            var contextKey = context.Key;
            var actorContext = context.ActorContext;

            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                simulationState.CopyFrom(actorContext.WorldState);

                var interruptsMethod = await methodSelector.SelectInterruptsAsync(
                    methods,
                    currentMethod,
                    simulationState,
                    cancellationToken);

                if (interruptsMethod != null)
                {
                    DebugLogger.LogPlanFailed(contextKey, $"Interrupt: {Name} switching to {interruptsMethod.Name}");
                    break;
                }
            }
        }

        UniTask<Method<TActorContext, TWorldState>> SelectCurrentMethodAsync(
           TWorldState worldState,
           CancellationToken cancellationToken)
        {
            return methodSelector.SelectAsync(methods, worldState, cancellationToken);
        }
    }
}
