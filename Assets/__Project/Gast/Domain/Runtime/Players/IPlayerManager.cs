using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Characters;

namespace Gast.Domain.Players
{
    public interface IPlayerManager
    {
        ILive<ICharacter> CurrentCharacter { get; }

        void Possess(CharacterId characterId);

        void Unpossess();

        /// <summary>
        /// Temporarily attach an AI brain to the current player character.
        /// The PlayerBrain is stored for later restoration.
        /// </summary>
        /// <param name="aiBrain">The AI brain to attach</param>
        /// <returns>True if takeover successful, false if no character or already under AI control</returns>
        bool TakeoverWithAI(ICharacterBrain aiBrain);

        /// <summary>
        /// Restore the original PlayerBrain to the current character.
        /// </summary>
        /// <returns>True if restoration successful, false if not under AI control</returns>
        bool RestorePlayerControl();

        /// <summary>
        /// Set the AI goal controller to monitor for automatic restoration.
        /// Pass null to stop monitoring.
        /// </summary>
        /// <param name="goalController">The goal controller to monitor, or null to stop</param>
        void SetAIMonitorTarget(IAIGoalController goalController);
    }
}