using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Core.Tasks;
using Gast.Domain.Characters;
using Cryst.Domain.Characters;
using R3;

namespace Cryst.Infrastructure.EventHandlers
{
    public class CharacterDecompositionHandler : ILifecycleTask
    {
        readonly IDomainEventSubscriber eventSubscriber;

        public CharacterDecompositionHandler(IDomainEventSubscriber eventSubscriber)
        {
            this.eventSubscriber = eventSubscriber;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            eventSubscriber.Subscribe<CharacterDefeatedEvent>(OnCharacterDefeated)
                .AddTo(cancellationToken);
            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        void OnCharacterDefeated(CharacterDefeatedEvent e)
        {
            DestroyCharacterAfterDelay(e.DefeatedCharacter.Character).Forget();
        }

        async UniTaskVoid DestroyCharacterAfterDelay(ICharacter character)
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(5),
                cancellationToken: character.CancellationToken);

            character.Destroy();
        }
    }
}