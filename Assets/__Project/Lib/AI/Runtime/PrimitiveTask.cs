using Cysharp.Threading.Tasks;
using System.Threading;

namespace Gast.Lib.AI
{
    public abstract class PrimitiveTask<TWorldState, TContext> : ITask<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        public string Name { get; }

        protected PrimitiveTask(string name) => Name = name;

        public UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            if (!CanExecute(worldState))
                return UniTask.FromResult(false);

            if (options.MaxDepth != 0)
            {
                Simulate(worldState);
            }

            return UniTask.FromResult(true);
        }

        public UniTask RunAsync(TContext ctx, CheckOptions? options)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            DebugLogger.LogExecutingAction(Name);
            return ExecuteAsync(ctx);
        }

        protected abstract void Simulate(TWorldState worldState);

        protected abstract bool CanExecute(TWorldState worldState);

        protected abstract UniTask ExecuteAsync(TContext ctx);
    }
}