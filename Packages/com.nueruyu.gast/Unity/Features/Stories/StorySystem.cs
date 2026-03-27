using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using Gast.Unity.Infrastructure.Stories;
using UnityEngine;

namespace Gast.Unity.Features.Stories
{
    /// <summary>
    /// Executes HTN story domains on demand. Starting a new story cancels any currently running one.
    /// </summary>
    public class StorySystem : IStoryRunner, IDisposable
    {
        readonly DynamicStoryDomainFactory domainFactory;
        readonly StoryActorContext actorContext;

        CancellationTokenSource storyCts;

        public StorySystem(
            DynamicStoryDomainFactory domainFactory,
            StoryActorContext actorContext)
        {
            this.domainFactory = domainFactory;
            this.actorContext = actorContext;
        }

        public void StartStory(string storyJson)
        {
            storyCts?.Cancel();
            storyCts?.Dispose();
            storyCts = new CancellationTokenSource();
            RunStoryAsync(storyJson, storyCts.Token).Forget();
        }

        public void Dispose()
        {
            storyCts?.Cancel();
            storyCts?.Dispose();
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
            catch (OperationCanceledException)
            {
                Debug.Log("[StorySystem] Story cancelled.");
            }
        }
    }
}
