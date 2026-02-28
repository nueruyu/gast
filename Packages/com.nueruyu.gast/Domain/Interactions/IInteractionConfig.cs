namespace Gast.Domain.Interactions
{
    /// <summary>
    /// Interface for interaction configuration.
    /// Can be implemented by plain classes, Serializable classes, or ScriptableObjects.
    /// </summary>
    public interface IInteractionConfig
    {
        /// <summary>
        /// Display text for the interaction (e.g., "Pickup", "Power On").
        /// </summary>
        string Prompt { get; }

        /// <summary>
        /// Key binding hint (e.g., "E").
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Type of interaction: instant (press) or hold.
        /// </summary>
        InteractionType Type { get; }

        /// <summary>
        /// Duration in seconds for hold interactions. Ignored for instant interactions.
        /// </summary>
        float HoldDuration { get; }
    }
}
