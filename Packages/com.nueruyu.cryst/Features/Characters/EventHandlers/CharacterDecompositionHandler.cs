using System;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Characters;

namespace Cryst.Features.Characters.EventHandlers
{
    public class CharacterDecompositionHandler
    {
        public void Handle(CharacterDefeatedEvent e)
        {
            DestroyCharacterAfterDelay(e.DefeatedCharacter).Forget();
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