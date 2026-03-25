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
        readonly PlanningStateStore interruptValidationStore = new();
        readonly Method<TActorContext, TWorldState>[] methods;
        readonly IMethodSelector<TActorContext, TWorldState> methodSelector;
        readonly PlanningStateStore planningStateStore = new();
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

            if (method == null)
                return;

            // Restore state to pre-selection so subtask SimulateAsync applies effects exactly once.
            simulationState.WriteTo(ref worldState);

            if (!context.PlanFound)
                context.RootMethodName = method.Name;

            foreach (var task in method.SubTasks)
                await task.SimulateAsync(context, cancellationToken);
        }

        public async UniTask RunAsync(
            ExecutionContext<TActorContext> context,
            CancellationToken cancellationToken)
        {
            planningStateStore.Clear();
            interruptValidationStore.Clear();

            DebugLogger.EnterTask(context.Key, Name);
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    context.ActorContext.WorldState.WriteTo(ref simulationState);

                    var validationContext = new ValidationContext<TWorldState>(simulationState, planningStateStore);

                    var method = await SelectCurrentMethodAsync(validationContext, cancellationToken);
                    if (method == null)
                    {
                        DebugLogger.LogPlanFailed(context.Key, $"No valid method for {Name}");
                        break;
                    }

                    DebugLogger.LogMethodSelected(context.Key, Name, method.Name, context.ActorContext.WorldState);
                    DebugLogger.LogPlan(context.Key, method.SubTasks);

                    using var localCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                    var currentMethodInfo = new CurrentMethodInfo<TActorContext, TWorldState>(method);

                    var runMethodTask = RunMethodAsync(currentMethodInfo, context, localCts.Token);
                    var monitorTask = MonitorInterruptsAsync(currentMethodInfo, context, interruptValidationStore,
                        localCts.Token);

                    var completedTaskIndex = await UniTask.WhenAny(runMethodTask, monitorTask);

                    localCts.Cancel();

                    // Method completed, exit loop to return control to parent.
                    if (completedTaskIndex == 0) break;

                    // Interrupt occurred, re-plan in the next loop iteration.
                    planningStateStore.CopyFrom(interruptValidationStore);
                }
            }
            finally
            {
                DebugLogger.ExitTask(context.Key);
            }
        }

        async UniTask RunMethodAsync(
            CurrentMethodInfo<TActorContext, TWorldState> currentMethodInfo,
            ExecutionContext<TActorContext> context,
            CancellationToken cancellationToken)
        {
            var subTasks = currentMethodInfo.Method.SubTasks;
            for (var i = 0; i < subTasks.Count; i++)
            {
                currentMethodInfo.NextSubTaskIndex = i;
                var task = subTasks[i];
                await task.RunAsync(context, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                context.ActorContext.UpdateWorldState();
            }

            currentMethodInfo.NextSubTaskIndex = subTasks.Count; // Mark as completed
        }

        async UniTask MonitorInterruptsAsync(
            CurrentMethodInfo<TActorContext, TWorldState> currentMethodInfo,
            ExecutionContext<TActorContext> context,
            PlanningStateStore planningStateStore,
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                if (currentMethodInfo.NextSubTaskIndex >=
                    currentMethodInfo.Method.SubTasks.Count) break; // Method completed, stop monitoring

                var currentlyExecutingTask =
                    currentMethodInfo.Method.SubTasks[currentMethodInfo.NextSubTaskIndex];

                context.ActorContext.WorldState.WriteTo(ref simulationState);

                var validationContext =
                    new ValidationContext<TWorldState>(simulationState, planningStateStore, currentlyExecutingTask);

                var interruptsMethod = await methodSelector.SelectInterruptsAsync(
                    methods,
                    currentMethodInfo,
                    validationContext,
                    cancellationToken);

                if (interruptsMethod != null)
                {
                    DebugLogger.Log(context.Key,
                        $"Method interrupted - [{Name}] {currentMethodInfo.Method.Name} -> {interruptsMethod.Name}");
                    return; // Interrupt detected, complete this task.
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