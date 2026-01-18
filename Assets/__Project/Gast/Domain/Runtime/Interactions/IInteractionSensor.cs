using System.Collections.Generic;

namespace Gast.Domain.Interactions
{
    /// <summary>
    /// Interface for a sensor that detects nearby interactable objects.
    /// </summary>
    public interface IInteractionSensor
    {
        /// <summary>
        /// Gets a list of currently visible/detectable interactable objects.
        /// </summary>
        IReadOnlyList<IInteractable> DetectableInteractables { get; }

        /// <summary>
        /// Checks if a specific interactable is currently detectable.
        /// </summary>
        bool IsDetectable(InteractableId id);
    }
}
