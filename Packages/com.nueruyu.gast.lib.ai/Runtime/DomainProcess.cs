using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Lib.AI
{
    public interface IDomainProcess
    {
        UniTask RunAsync(CancellationToken cancellationToken);
        void UpdateState();
    }

    public class DomainProcess<TWorldState, TContext> : IDomainProcess
        where TWorldState : class, IWorldState<TWorldState>, new()
        where TContext : struct, IContext<TContext, TWorldState>
    {
        readonly TContext context;
        readonly AIRunner<TWorldState, TContext> runner;

        public DomainProcess(AIRunner<TWorldState, TContext> runner, TContext context)
        {
            this.runner = runner;
            this.context = context;
        }

        public async UniTask RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                await runner.RunAsync(context.WithCancellationToken(cancellationToken));
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
        public void UpdateState()
        {
            context.UpdateWorldState();
        }
    }
}