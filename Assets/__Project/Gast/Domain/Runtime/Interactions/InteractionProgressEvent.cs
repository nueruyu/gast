using Gast.Domain.Characters;

namespace Gast.Domain.Interactions
{
    /// <summary>
    /// Event data for hold interaction progress updates.
    /// </summary>
    public readonly struct InteractionProgressEvent
    {
        public CharacterId InteractorId { get; }
        public InteractableId InteractableId { get; }
        public float Progress { get; } // 0.0 to 1.0

        public InteractionProgressEvent(CharacterId interactorId, InteractableId interactableId, float progress)
        {
            InteractorId = interactorId;
            InteractableId = interactableId;
            Progress = progress;
        }
    }
}
