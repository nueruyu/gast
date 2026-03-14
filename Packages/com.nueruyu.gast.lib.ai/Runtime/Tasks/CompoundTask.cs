using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Debugging;
using Gast.Lib.AI.Testing;

namespace Gast.Lib.AI.Tasks
{
    public class CompoundTask<TActorContext, TWorldState> : ITask<TActorContext, TWorldState>
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly Method<TActorContext, TWorldState>[] methods;
        readonly IMethodSelector<TActorContext, TWorldState> methodSelector;
        readonly PlanningStateStore monitoringPlanningStateStore = new();
        TWorldState simulationState;

        internal CompoundTask(
            string name,
            IEnumerable<Method<TActorContext, TWorldState>> methods,
            IMethodSelector<TActorContext, TWorldState> methodSelector)
        {
            Name = name;
            this.methods = methods.ToArray();
            this.methodSelector = methodSelector;
        }

        public string Name { get; }

        public async UniTask<bool> ValidateAsync(
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            var method = await SelectCurrentMethodAsync(
                context,
                cancellationToken);

            return method != null;
        }

        public async UniTask SimulateAsync(SimulationContext<TWorldState> context, CancellationToken cancellationToken)
        {
            var worldState = context.WorldState;
            worldState.WriteTo(ref simulationState);

            var planningContext = new PlanningStateStore();
            var validationContext = new ValidationContext<TWorldState>(simulationState, planningContext);

            var method = await methodSelector.SelectAsync(methods, validationContext, cancellationToken);

            if (method == null) return;

            // Restore state to pre-selection so subtask SimulateAsync applies effects exactly once.
            simulationState.WriteTo(ref worldState);

            if (!context.PlanFound) context.RootMethodName = method.Name;

            foreach (var task in method.SubTasks) await task.SimulateAsync(context, cancellationToken);
        }

        public async UniTask RunAsync(ExecutionContext<TActorContext> context, CancellationToken cancellationToken)
        {
            DebugLogger.EnterTask(context.Key, Name);

            try
            {
                context.ActorContext.WorldState.WriteTo(ref simulationState);
                var validationContext = new ValidationContext<TWorldState>(simulationState, context.PlanningStateStore);

                var method = await SelectCurrentMethodAsync(
                    validationContext,
                    cancellationToken);

                if (method == null)
                {
                    DebugLogger.LogPlanFailed(context.Key, $"No valid method for {Name}");
                    return;
                }

                DebugLogger.LogMethodSelected(context.Key, Name, method.Name, context.ActorContext.WorldState);
                DebugLogger.LogPlan(context.Key, method.SubTasks);

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
                DebugLogger.ExitTask(context.Key);
            }
        }

        async UniTask RunMethodAsync(
            Method<TActorContext, TWorldState> method,
            ExecutionContext<TActorContext> context,
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
            ExecutionContext<TActorContext> context,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                monitoringPlanningStateStore.Clear();

                context.ActorContext.WorldState.WriteTo(ref simulationState);
                var validationContext =
                    new ValidationContext<TWorldState>(simulationState, monitoringPlanningStateStore);

                var interruptsMethod = await methodSelector.SelectInterruptsAsync(
                    methods,
                    currentMethod,
                    validationContext,
                    cancellationToken);

                if (interruptsMethod != null)
                {
                    DebugLogger.LogPlanFailed(context.Key, $"Interrupt: {Name} switching to {interruptsMethod.Name}");
                    break;
                }
            }
        }

        UniTask<Method<TActorContext, TWorldState>> SelectCurrentMethodAsync(
            ValidationContext<TWorldState> context,
            CancellationToken cancellationToken)
        {
            return methodSelector.SelectAsync(methods, context, cancellationToken);
        }
    }
}