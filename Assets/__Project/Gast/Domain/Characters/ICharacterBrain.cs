using System;

namespace Gast.Domain.Characters
{
    /// <summary>
    /// Interface for character decision-making systems.
    /// Brains control characters by accessing their controller.
    /// </summary>
    public interface ICharacterBrain
    {
        /// <summary>
        /// Called when this brain is attached to a character.
        /// Use this to start behavior loops and update camera targets.
        /// </summary>
        void OnAttached(ICharacter character);

        /// <summary>
        /// Called when this brain is detached from a character.
        /// Use this to clean up resources and cancel running operations.
        /// </summary>
        void OnDetached();
    }
}