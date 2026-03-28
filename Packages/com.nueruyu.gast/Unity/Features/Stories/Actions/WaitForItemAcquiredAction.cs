using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Domain.AI.Attributes;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Lib.AI;

namespace Gast.Unity.Features.Stories.Actions
{
    [AIAction("WaitForItemAcquired")]
    public class WaitForItemAcquiredAction : IAction<StoryActorContext, StoryWorldState>
    {
        readonly CharacterId acquirerCharacterId;
        readonly ItemId itemId;

        public WaitForItemAcquiredAction(CharacterId acquirerCharacterId, ItemId itemId)
        {
            this.acquirerCharacterId = acquirerCharacterId;
            this.itemId = itemId;
        }

        public bool IsAvailable(StoryWorldState worldState) => true;

        public void Simulate(StoryWorldState worldState) { }

        public async UniTask ExecuteAsync(StoryActorContext context, CancellationToken cancellationToken)
        {
            var tcs = new UniTaskCompletionSource();

            using var subscription = context.EventSubscriber.Subscribe<ItemAcquiredEvent>(e =>
            {
                if (e.AcquirerId == acquirerCharacterId && e.AcquiredItemId == itemId)
                    tcs.TrySetResult();
            });

            await using var ctr = cancellationToken.Register(() => tcs.TrySetCanceled());

            await tcs.Task;
        }
    }
}
