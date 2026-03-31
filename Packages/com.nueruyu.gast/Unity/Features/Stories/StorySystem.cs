using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Application.AIPlanning;
using Gast.Lib.AI;
using Gast.Lib.AI.Debugging;
using UnityEngine;

namespace Gast.Unity.Features.Stories
{
    /// <summary>
    ///     Executes HTN story domains on demand. Starting a new story cancels any currently running one.
    /// </summary>
    public class StorySystem : IStoryRunner, IDisposable
    {
        readonly StoryActorContext actorContext;
        readonly IContextRegistry contextRegistry;
        readonly StoryDomainFactory domainFactory;
        ContextKey? activeContextKey;

        CancellationTokenSource storyCts;

        public StorySystem(
            StoryDomainFactory domainFactory,
            StoryActorContext actorContext,
            IContextRegistry contextRegistry)
        {
            this.domainFactory = domainFactory;
            this.actorContext = actorContext;
            this.contextRegistry = contextRegistry;
        }

        public void Dispose()
        {
            CleanupActiveStory();
        }

        public void StartStory(StoryBlueprint blueprint)
        {
            CleanupActiveStory();
            storyCts = new CancellationTokenSource();
            RunStoryAsync(blueprint, storyCts.Token).Forget();
        }

        void CleanupActiveStory()
        {
            storyCts?.Cancel();
            storyCts?.Dispose();
            storyCts = null;

            if (activeContextKey.HasValue)
            {
                contextRegistry.Unregister(activeContextKey.Value);
                activeContextKey = null;
            }
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

                activeContextKey = contextKey;
                contextRegistry.Register(contextKey, actorContext.WorldState, "Story Teller");

                var process = new DomainProcess<StoryActorContext, StoryWorldState>(brain, actorContext, contextKey);
                await process.RunAsync(cancellationToken);

                Debug.Log($"[StorySystem] Story domain '{domainName}' completed.");
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[StorySystem] Story cancelled.");
            }
            finally
            {
                if (activeContextKey.HasValue)
                {
                    contextRegistry.Unregister(activeContextKey.Value);
                    activeContextKey = null;
                }
            }
        }
    }
}