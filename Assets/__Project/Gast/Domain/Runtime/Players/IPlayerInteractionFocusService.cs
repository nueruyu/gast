using Gast.Core.Observables;
using Gast.Domain.Interactions;

namespace Gast.Domain.Players
{
    /// <summary>
    /// Service that determines the single interactable object the player is focusing on.
    /// </summary>
    public interface IPlayerInteractionFocusService
    {
        /// <summary>
        /// Gets an observable property for the currently focused interactable.
        /// This will be null if no interactable is in focus.
        /// </summary>
        ILive<IInteractable> FocusedInteractable { get; }
    }
}
