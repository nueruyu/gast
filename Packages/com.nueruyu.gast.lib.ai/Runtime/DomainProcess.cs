using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Gast.Lib.AI
{
    public interface IDomainProcess
    {
        UniTask RunAsync(CancellationToken cancellationToken);
        void UpdateState();
    }

    public class DomainProcess<TActorContext, TWorldState> : IDomainProcess
        where TWorldState : class, IWorldState<TWorldState>
        where TActorContext : class, IActorContext<TWorldState>
    {
        readonly AIContext<TActorContext> context;
        readonly AIRunner<TActorContext, TWorldState> runner;

        public DomainProcess(AIRunner<TActorContext, TWorldState> runner, AIContext<TActorContext> context)
        {
            this.runner = runner;
            this.context = context;
        }

        public async UniTask RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await runner.RunAsync(context, cancellationToken);
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        public void UpdateState()
        {
            context.ActorContext.UpdateWorldState();
        }
    }
}
