using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Debugging;
using System.Threading;

namespace Gast.Lib.AI.Tasks
{
    public class PrimitiveTask<TWorldState, TContext> : ITask<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly IAction<TWorldState, TContext> action;

        public string Name { get; }

        public PrimitiveTask(string name, IAction<TWorldState, TContext> action)
        {
            Name = name;
            this.action = action;
        }

        public UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            if (!action.CanExecute(worldState))
                return UniTask.FromResult(false);

            action.Simulate(worldState);

            return UniTask.FromResult(true);
        }

        public async UniTask RunAsync(TContext ctx)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            var contextKey = ctx.ContextKey;

            DebugLogger.EnterTask(contextKey, Name);
            try
            {
                await action.ExecuteAsync(ctx);
            }
            finally
            {
                DebugLogger.ExitTask(contextKey);
            }
        }
    }

    public class ParametricPrimitiveTask<TWorldState, TContext, TParam> : ITask<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly IAction<TWorldState, TContext, TParam> action;
        readonly TParam param;

        public string Name { get; }

        public ParametricPrimitiveTask(string name, IAction<TWorldState, TContext, TParam> action, TParam param)
        {
            Name = name;
            this.action = action;
            this.param = param;
        }

        public UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            if (!action.CanExecute(worldState, param))
                return UniTask.FromResult(false);

            action.Simulate(worldState, param);

            return UniTask.FromResult(true);
        }

        public async UniTask RunAsync(TContext ctx)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();

            var contextKey = ctx.ContextKey;

            DebugLogger.EnterTask(contextKey, $"{Name}({param})");
            try
            {
                await action.ExecuteAsync(ctx, param);
            }
            finally
            {
                DebugLogger.ExitTask(contextKey);
            }
        }
    }
}