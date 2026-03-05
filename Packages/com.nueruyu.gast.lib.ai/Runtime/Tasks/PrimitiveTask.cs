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

        public string Name => action.ToString();

        public PrimitiveTask(IAction<TWorldState, TContext> action)
        {
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
}
