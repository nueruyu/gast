using Cysharp.Threading.Tasks;
using System.Threading;

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
        readonly AIRunner<TWorldState, TContext> runner;
        readonly TContext context;

        public DomainProcess(AIRunner<TWorldState, TContext> runner, TContext context)
        {
            this.runner = runner;
            this.context = context;
        }

        public async UniTask RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await runner.RunAsync(context.WithCancellationToken(cancellationToken));
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        public void UpdateState() => context.UpdateWorldState();
    }
}
