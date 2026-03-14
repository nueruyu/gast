using System.Threading;
using Cysharp.Threading.Tasks;

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
        readonly TActorContext actorContext;
        readonly ContextKey contextKey;
        readonly AIDomain<TActorContext, TWorldState> domain;

        public DomainProcess(
            AIDomain<TActorContext, TWorldState> domain,
            TActorContext actorContext,
            ContextKey contextKey)
        {
            this.domain = domain;
            this.actorContext = actorContext;
            this.contextKey = contextKey;
        }

        public async UniTask RunAsync(CancellationToken cancellationToken)
        {
            var planningContext = new PlanningStateStore();
            while (!cancellationToken.IsCancellationRequested)
            {
                await domain.RootTask.RunAsync(
                    new ExecutionContext<TActorContext>(contextKey, actorContext, planningContext),
                    cancellationToken);

                planningContext.Clear();
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        public void UpdateState()
        {
            actorContext.UpdateWorldState();
        }
    }
}