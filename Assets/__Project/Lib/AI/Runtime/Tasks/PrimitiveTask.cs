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

        public UniTask RunAsync(TContext ctx)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            DebugLogger.LogExecutingAction(Name);
            return action.ExecuteAsync(ctx);
        }
    }
}
