using UnityEngine;
using Gast.Core.Observables;
using Gast.Features.Characters;
using Gast.Domain.Characters;
using Gast.Domain.Players;
using Gast.Domain.AI;

namespace GastGame.Features.Players
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
        readonly Live<ICharacterAIBrain> currentAIBrain = new(null);

        public PlayerManager(
            PlayerBrain playerBrain,
            ICharacterActorRepository characterActorRepository)
        {
            this.playerBrain = playerBrain;
            this.characterActorRepository = characterActorRepository;
            CurrentCharacter = currentCharacter.Cast<Character, ICharacter>();
        }

        /// <summary>
        /// Observable property for the character currently possessed by the player.
        /// </summary>
        public ILive<ICharacter> CurrentCharacter { get; }

        public ILive<ICharacterAIBrain> CurrentAIBrain => currentAIBrain;

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

            currentAIBrain.Value = null;
        }

        /// <summary>
        /// Temporarily attach an AI brain to the current player character.
        /// The PlayerBrain is stored for later restoration.
        /// </summary>
        public bool TakeoverWithAI(ICharacterAIBrain aiBrain)
        {
            if (currentCharacter.Value == null)
            {
                Debug.LogWarning("PlayerManager: Cannot takeover - no character possessed");
                return false;
            }

            currentCharacter.Value.AttachBrain(aiBrain);
            currentAIBrain.Value = aiBrain;

            Debug.Log($"PlayerManager: AI took over control of {currentCharacter.Value}");
            return true;
        }

        /// <summary>
        /// Restore the original PlayerBrain to the current character.
        /// </summary>
        public bool RestorePlayerControl()
        {
            if (currentCharacter.Value == null)
            {
                Debug.LogWarning("PlayerManager: Cannot restore - no character possessed");
                return false;
            }

            currentCharacter.Value.AttachBrain(playerBrain);
            currentAIBrain.Value = null;

            Debug.Log($"PlayerManager: Restored player control of {currentCharacter.Value}");
            return true;
        }
    }
}