using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Gast.Lib.AI
{
    public class CompoundTask<TWorldState> : ITask<TWorldState>
        where TWorldState : struct
    {
        public string Name { get; }
        public List<Method<TWorldState>> Methods { get; } = new();
        public IMethodSelector<TWorldState> Selector { get; set; } = new Selectors.PrioritySelector<TWorldState>();
        public int LocalDepthLimit { get; set; } = -1;

        public CompoundTask(string name) => Name = name;

        public async UniTask<(bool, TWorldState)> ValidateAsync(
            TWorldState state,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            var (method, resultState) = await SelectCurrentMethodAsync(
                state,
                options,
                cancellationToken);

            var success = method != null;
            return (success, resultState);
        }

        public async UniTask RunAsync(Context<TWorldState> ctx, CheckOptions? options)
        {
            var (method, _) = await SelectCurrentMethodAsync(
                ctx.PlanState,
                options,
                ctx.CancellationToken);

            if (method == null)
            {
                DebugLogger.LogPlanFailed($"No valid method for {Name}");
                return;
            }

            DebugLogger.LogMethodSelected(Name, method.Name, ctx.PlanState);

            using var localCts = CancellationTokenSource.CreateLinkedTokenSource(ctx.CancellationToken);

            try
            {
                await UniTask.WhenAny(
                    RunMethodAsync(method, ctx, localCts.Token),
                    MonitorInterruptsAsync(method, ctx, options, localCts.Token)
                );
            }
            finally
            {
                localCts.Cancel();
            }
        }

        async UniTask RunMethodAsync(
            Method<TWorldState> method,
            Context<TWorldState> ctx,
            CancellationToken cancellationToken)
        {
            foreach (var task in method.SubTasks)
            {
                var subCtx = ctx.WithCancellationToken(cancellationToken);
                await task.RunAsync(subCtx);
            }
        }

        async UniTask MonitorInterruptsAsync(
            Method<TWorldState> currentMethod,
            Context<TWorldState> ctx,
            CheckOptions? options,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                var interruptsMethod = await Selector.SelectInterruptsAsync(
                    Methods,
                    currentMethod,
                    ctx.CurrentState,
                    GetEffectiveOptions(options),
                    cancellationToken);

                if (interruptsMethod != null)
                {
                    DebugLogger.LogPlanFailed($"Interrupt: {Name} switching to {interruptsMethod.Name}");
                    break;
                }
            }
        }

        UniTask<(Method<TWorldState>, TWorldState)> SelectCurrentMethodAsync(
           TWorldState state,
           CheckOptions? options,
           CancellationToken cancellationToken)
        {
            return Selector.SelectAsync(Methods, state, GetEffectiveOptions(options), cancellationToken);
        }

        CheckOptions GetEffectiveOptions(CheckOptions? options)
        {
            return CheckOptions.Resolve(
                options ?? CheckOptions.Deep,
                LocalDepthLimit);
        }
    }
}