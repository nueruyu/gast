using Gast.Domain.Cameras;
using Gast.Domain.Characters;
using Gast.Domain.Inputs;

namespace Gast.Domain.Players
{
    /// <summary>
    /// Handles player input and translates it into specific character actions.
    /// This decouples the generic PlayerBrain from game-specific character abilities.
    /// </summary>
    public interface IPlayerCharacterController
    {
        /// <summary>
        /// Processes input for the given character on each frame.
        /// </summary>
        /// <param name="character">The character to control.</param>
        void HandleInput(ICharacter character);
    }
}