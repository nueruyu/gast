using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Stories;
using Gast.Lib.AI;
using UnityEngine;

namespace Cryst.Features.Stories
{
    /// <summary>
    /// Lifecycle task that listens for <see cref="StoryGeneratedEvent"/> and runs the
    /// resulting HTN domain as a <see cref="DomainProcess{TActorContext,TWorldState}"/>.
    /// Starting a new story cancels any currently running one.
    /// </summary>
    public class StorySystem : ILifecycleTask
    {
        readonly IDomainEventSubscriber eventSubscriber;
        readonly DynamicStoryDomainFactory domainFactory;
        readonly StoryActorContext actorContext;

        CancellationTokenSource storyCts;

        public StorySystem(
            IDomainEventSubscriber eventSubscriber,
            DynamicStoryDomainFactory domainFactory,
            StoryActorContext actorContext)
        {
            this.eventSubscriber = eventSubscriber;
            this.domainFactory = domainFactory;
            this.actorContext = actorContext;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            eventSubscriber.Subscribe<StoryGeneratedEvent>(HandleStoryGenerated).AddTo(cancellationToken);
            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void HandleStoryGenerated(StoryGeneratedEvent e)
        {
            storyCts?.Cancel();
            storyCts?.Dispose();
            storyCts = new CancellationTokenSource();
            RunStoryAsync(e.StoryJson, storyCts.Token).Forget();
        }

        async UniTaskVoid RunStoryAsync(string storyJson, CancellationToken cancellationToken)
        {
            try
            {
                var domain = domainFactory.CreateDomain(storyJson);
                var domainName = domain.RootTask.Name;
                Debug.Log($"[StorySystem] Starting story domain: {domainName}");

                var brain = new StorytellerBrain(domain);
                var contextKey = new ContextKey("storyteller", domainName);
                var process = new DomainProcess<StoryActorContext, StoryWorldState>(brain, actorContext, contextKey);
                await process.RunAsync(cancellationToken);

                Debug.Log($"[StorySystem] Story domain '{domainName}' completed.");
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log("[StorySystem] Story cancelled.");
            }
        }
    }
}
