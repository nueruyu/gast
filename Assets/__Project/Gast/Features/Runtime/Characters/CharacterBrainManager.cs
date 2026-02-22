using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Gast.Core.Tasks;
using Gast.Domain.AI;
using Gast.Domain.Characters;
using R3;

namespace Gast.Features.Characters
{
    public class CharacterBrainManager : ICharacterBrainManager, ILifecycleTask
    {
        readonly ICharacterRepository characterRepository;
        readonly Dictionary<CharacterId, ICharacterBrain> activeBrains = new();

        public CharacterBrainManager(ICharacterRepository characterRepository)
        {
            this.characterRepository = characterRepository;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            characterRepository.Unregistered
                .Subscribe(OnCharacterUnregistered)
                .AddTo(cancellationToken);
            await UniTask.WaitUntilCanceled(cancellationToken);
        }

        public void AttachBrain(CharacterId characterId, ICharacterBrain newBrain)
        {
            DetachBrain(characterId);

            var character = characterRepository.Get(characterId);
            activeBrains[characterId] = newBrain;
            newBrain?.OnAttached(character);
        }

        public void DetachBrain(CharacterId characterId)
        {
            if (activeBrains.Remove(characterId, out var oldBrain))
            {
                oldBrain?.OnDetached();
            }
        }

        void OnCharacterUnregistered(ICharacter character)
        {
            DetachBrain(character.Id);
        }
    }
}
