using System;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Cryst.Domain.Characters;

namespace Cryst.Modules.Characters.EventHandlers
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