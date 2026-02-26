namespace Gast.Domain.Interactions
{
    /// <summary>
    /// Type of interaction.
    /// </summary>
    public enum InteractionType
    {
        /// <summary>
        /// Instant interaction on key press.
        /// </summary>
        Instant,

        /// <summary>
        /// Requires holding the key for a duration.
        /// </summary>
        Hold
    }
}
