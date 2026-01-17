using Cysharp.Threading.Tasks;
using Gast.Lib.AI.Debugging;
using System.Threading;

namespace Gast.Lib.AI.Tasks
{
    public abstract class PrimitiveTask<TWorldState, TContext> : ITask<TWorldState, TContext>
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        public string Name { get; }

        protected PrimitiveTask(string name)
        {
            Name = name;
        }

        public UniTask<bool> ValidateAsync(
            TWorldState worldState,
            CancellationToken cancellationToken)
        {
            if (!CanExecute(worldState))
                return UniTask.FromResult(false);

            Simulate(worldState);

            return UniTask.FromResult(true);
        }

        public UniTask RunAsync(TContext ctx)
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