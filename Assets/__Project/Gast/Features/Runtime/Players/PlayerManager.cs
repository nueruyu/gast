using UnityEngine;
using Gast.Core.Observables;
using Gast.Features.Characters;
using Gast.Domain.Characters;
using Gast.Domain.Players;
using Gast.Domain.AI;
using Gast.Core.Tasks;
using System.Threading.Tasks;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Gast.Features.Players
{
    /// <summary>
    /// Manages player possession of different characters.
    /// Provides observable access to the currently controlled character.
    /// </summary>
    public class PlayerManager : IPlayerManager, ILifecycleTask
    {
        readonly PlayerBrain playerBrain;
        readonly ICharacterActorRepository characterActorRepository;
        readonly AIControlMonitor aiControlMonitor;
        readonly Live<Character> currentCharacter = new(null);

        ICharacterBrain temporaryBrain;
        bool isUnderAIControl;

        public PlayerManager(
            PlayerBrain playerBrain,
            ICharacterActorRepository characterActorRepository,
            AIControlMonitor aiControlMonitor)
        {
            this.playerBrain = playerBrain;
            this.characterActorRepository = characterActorRepository;
            this.aiControlMonitor = aiControlMonitor;
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

            if (isUnderAIControl)
            {
                aiControlMonitor.SetTarget(null);
                isUnderAIControl = false;
                temporaryBrain = null;
            }
        }

        /// <summary>
        /// Temporarily attach an AI brain to the current player character.
        /// The PlayerBrain is stored for later restoration.
        /// </summary>
        public bool TakeoverWithAI(ICharacterBrain aiBrain)
        {
            if (currentCharacter.Value == null)
            {
                Debug.LogWarning("PlayerManager: Cannot takeover - no character possessed");
                return false;
            }

            if (isUnderAIControl)
            {
                Debug.LogWarning("PlayerManager: Cannot takeover - already under AI control");
                return false;
            }

            temporaryBrain = aiBrain;
            isUnderAIControl = true;

            currentCharacter.Value.AttachBrain(aiBrain);

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

            if (!isUnderAIControl)
            {
                Debug.LogWarning("PlayerManager: Cannot restore - not under AI control");
                return false;
            }

            aiControlMonitor.SetTarget(null);

            currentCharacter.Value.AttachBrain(playerBrain);

            isUnderAIControl = false;
            temporaryBrain = null;

            Debug.Log($"PlayerManager: Restored player control of {currentCharacter.Value}");
            return true;
        }

        /// <summary>
        /// Set the AI goal controller to monitor for automatic restoration.
        /// Pass null to stop monitoring.
        /// </summary>
        public void SetAIMonitorTarget(IAIGoalController goalController)
        {
            aiControlMonitor.SetTarget(goalController);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (isUnderAIControl && aiControlMonitor.ShouldRestore())
                {
                    RestorePlayerControl();
                }

                await UniTask.NextFrame(cancellationToken);
            }
        }
    }
}