using UnityEngine;
using Gast.Core.Observables;
using Gast.Features.Characters;
using Gast.Domain.Characters;
using Gast.Domain.Players;

namespace Gast.Features.Players
{
    /// <summary>
    /// Manages player possession of different characters.
    /// Provides observable access to the currently controlled character.
    /// </summary>
    public class PlayerManager : IPlayerManager
    {
        readonly PlayerBrain playerBrain;
        readonly ICharacterActorRepository characterActorRepository;
        readonly Live<Character> currentCharacter = new(null);

        public PlayerManager(PlayerBrain playerBrain, ICharacterActorRepository characterActorRepository)
        {
            this.playerBrain = playerBrain;
            this.characterActorRepository = characterActorRepository;
            CurrentCharacter = currentCharacter.Cast<Character, ICharacter>();
        }

        /// <summary>
        /// Observable property for the character currently possessed by the player.
        /// </summary>
        public ILive<ICharacter> CurrentCharacter { get; }

        /// <summary>
        /// Switch player control to a different character.
        /// Detaches brain from current character and attaches to new one.
        /// </summary>
        public void Possess(CharacterId characterId)
        {
            Unpossess();

            var character = characterActorRepository.Get(characterId);
            character.AttachBrain(playerBrain);
            currentCharacter.Value = character;

            Debug.Log($"PlayerManager: Possessed {character}");
        }

        /// <summary>
        /// Release control of the current character.
        /// </summary>
        public void Unpossess()
        {
            if (currentCharacter.Value != null)
            {
                currentCharacter.Value.DetachBrain();
                currentCharacter.Value = null;
            }
        }
    }
}