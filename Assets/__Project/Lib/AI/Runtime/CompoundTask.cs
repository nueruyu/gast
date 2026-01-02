using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DescrioGames.Lib.AI
{
    public class CompoundTask<TWorldState> : ITask<TWorldState>
        where TWorldState : struct
    {
        public string Name { get; }
        public List<Method<TWorldState>> Methods { get; } = new List<Method<TWorldState>>();
        public IMethodSelector<TWorldState> Selector { get; set; } = new Selectors.PrioritySelector<TWorldState>();
        public int LocalDepthLimit { get; set; } = -1;
        public bool RunPlanningOnBackground { get; set; } = false;

        public CompoundTask(string name) => Name = name;

        public bool Validate(ref TWorldState state, CheckOptions options, ISimulationContext context, IEnvironmentModel<TWorldState> environment = null)
        {
            var myOptions = CheckOptions.Resolve(options, LocalDepthLimit);
            return Selector.Select(Methods, ref state, myOptions, context, out _);
        }

        public async UniTask RunAsync(Context<TWorldState> ctx)
        {
            Method<TWorldState> method = null;
            var simContext = new SimulationContext();
            bool success;

            if (RunPlanningOnBackground)
            {
                var planState = ctx.PlanState;
                success = await UniTask.RunOnThreadPool(() =>
                {
                    return Selector.Select(Methods, ref planState, CheckOptions.Deep, simContext, out method);
                });
            }
            else
            {
                var options = LocalDepthLimit == 0 ? CheckOptions.Shallow : CheckOptions.Deep;
                var planState = ctx.PlanState;
                success = Selector.Select(Methods, ref planState, options, simContext, out method);
            }

            if (!success || method == null)
            {
                DebugLogger.LogPlanFailed($"No valid method for {Name}");
                return;
            }

            DebugLogger.LogMethodSelected(Name, method.Name, ctx.PlanState);
            await UniTask.SwitchToMainThread();

            using var localCts = CancellationTokenSource.CreateLinkedTokenSource(ctx.Token);

            try
            {
                await UniTask.WhenAny(
                    RunMethodSequence(method, ctx, localCts.Token),
                    MonitorInterrupts(method, ctx, localCts, simContext)
                );
                localCts.Cancel();
            }
            catch (OperationCanceledException) { throw; }
        }

        async UniTask RunMethodSequence(Method<TWorldState> method, Context<TWorldState> ctx, CancellationToken token)
        {
            foreach (var subTask in method.SubTasks)
            {
                var subCtx = ctx.WithToken(token);
                await subTask.RunAsync(subCtx);
            }
        }

        async UniTask MonitorInterrupts(
            Method<TWorldState> currentMethod,
            Context<TWorldState> ctx,
            CancellationTokenSource cts,
            ISimulationContext simContext)
        {
            int currentPriority = Methods.IndexOf(currentMethod);

            if (currentPriority <= 0)
            {
                await UniTask.WaitUntilCanceled(cts.Token);
                return;
            }

            while (!cts.Token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);

                var currentState = ctx.CurrentState;

                for (int i = 0; i < currentPriority; i++)
                {
                    if (Methods[i].CheckCondition(currentState))
                    {
                        DebugLogger.LogPlanFailed($"Interrupt: {Name} switching to {Methods[i].Name}");
                        cts.Cancel();
                        return;
                    }
                }
            }
        }
    }
}