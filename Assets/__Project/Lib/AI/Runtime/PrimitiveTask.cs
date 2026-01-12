using Cysharp.Threading.Tasks;
using System.Threading;

namespace Gast.Lib.AI
{
    public abstract class PrimitiveTask<TWorldState> : ITask<TWorldState>
        where TWorldState : struct
    {
        public string Name { get; }

        protected PrimitiveTask(string name) => Name = name;

        public UniTask<(bool, TWorldState)> ValidateAsync(
            TWorldState state,
            CheckOptions options,
            CancellationToken cancellationToken)
        {
            if (!CanExecute(state))
                return UniTask.FromResult((false, state));

            if (options.MaxDepth != 0)
            {
                Simulate(ref state);
            }

            return UniTask.FromResult((true, state));
        }

        public UniTask RunAsync(Context<TWorldState> ctx, CheckOptions? options)
        {
            ctx.CancellationToken.ThrowIfCancellationRequested();
            DebugLogger.LogExecutingAction(Name);
            return ExecuteAsync(ctx);
        }

        protected abstract void Simulate(ref TWorldState state);

        protected abstract bool CanExecute(TWorldState state);

        protected abstract UniTask ExecuteAsync(Context<TWorldState> ctx);
    }
}