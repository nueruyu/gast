using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using UnityEngine;

namespace Gast.Unity.Features.Stories
{
    /// <summary>
    /// Executes HTN story domains on demand. Starting a new story cancels any currently running one.
    /// </summary>
    public class StorySystem : IStoryRunner, IDisposable
    {
        readonly StoryDomainFactory domainFactory;
        readonly StoryActorContext actorContext;

        CancellationTokenSource storyCts;

        public StorySystem(
            StoryDomainFactory domainFactory,
            StoryActorContext actorContext)
        {
            this.domainFactory = domainFactory;
            this.actorContext = actorContext;
        }

        public void StartStory(StoryBlueprint blueprint)
        {
            storyCts?.Cancel();
            storyCts?.Dispose();
            storyCts = new CancellationTokenSource();
            RunStoryAsync(blueprint, storyCts.Token).Forget();
        }

        public void Dispose()
        {
            storyCts?.Cancel();
            storyCts?.Dispose();
        }

        async UniTaskVoid RunStoryAsync(StoryBlueprint blueprint, CancellationToken cancellationToken)
        {
            try
            {
                var domain = domainFactory.CreateDomain(blueprint);
                var domainName = domain.RootTask.Name;
                Debug.Log($"[StorySystem] Starting story domain: {domainName}");

                var brain = new StorytellerBrain(domain);
                var contextKey = new ContextKey("storyteller", domainName);
                var process = new DomainProcess<StoryActorContext, StoryWorldState>(brain, actorContext, contextKey);
                await process.RunAsync(cancellationToken);

                Debug.Log($"[StorySystem] Story domain '{domainName}' completed.");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[StorySystem] Story cancelled.");
            }
        }
    }
}
