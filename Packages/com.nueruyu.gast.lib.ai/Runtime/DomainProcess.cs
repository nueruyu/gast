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
        readonly IDomainProvider<TActorContext, TWorldState> domainProvider;

        public DomainProcess(
            IDomainProvider<TActorContext, TWorldState> domainProvider,
            TActorContext actorContext,
            ContextKey contextKey)
        {
            this.domainProvider = domainProvider;
            this.actorContext = actorContext;
            this.contextKey = contextKey;
        }

        public async UniTask RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var executionContext = new ExecutionContext<TActorContext>(
                    contextKey,
                    actorContext
                );

                var domain = domainProvider.GetDomain();
                if (domain?.RootTask == null)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
                    continue;
                }

                await domain.RootTask.RunAsync(executionContext, cancellationToken);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }

        public void UpdateState()
        {
            actorContext.UpdateWorldState();
        }
    }
}