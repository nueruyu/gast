using System.Threading;
using Cryst.Domain.Characters;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Lib.AI;

namespace Cryst.Features.Stories.Actions
{
    public class WaitForCharacterDefeatedAction : IAction<StoryActorContext, StoryWorldState>
    {
        readonly CharacterId characterId;

        public WaitForCharacterDefeatedAction(CharacterId characterId)
        {
            this.characterId = characterId;
        }

        public bool IsAvailable(StoryWorldState worldState) => true;

        public void Simulate(StoryWorldState worldState) { }

        public async UniTask ExecuteAsync(StoryActorContext context, CancellationToken cancellationToken)
        {
            var tcs = new UniTaskCompletionSource();

            using var subscription = context.EventSubscriber.Subscribe<CharacterDefeatedEvent>(e =>
            {
                if (e.DefeatedCharacter.Id == characterId)
                    tcs.TrySetResult();
            });

            using var _ = cancellationToken.Register(() => tcs.TrySetCanceled());

            await tcs.Task;
        }
    }
}
