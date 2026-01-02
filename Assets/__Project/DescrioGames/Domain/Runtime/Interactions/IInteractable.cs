using UnityEngine;

namespace DescrioGames.Domain.Interactions
{
    /// <summary>
    /// Defines an object that can be interacted with.
    /// Pure data interface with no Unity dependencies and no behavior.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Configuration for this interaction (prompt, key, type, duration).
        /// </summary>
        IInteractionConfig Config { get; }

        /// <summary>
        /// Whether this interactable is currently available for interaction.
        /// </summary>
        bool CanInteract { get; }

        Vector3 Position { get; }
    }
}