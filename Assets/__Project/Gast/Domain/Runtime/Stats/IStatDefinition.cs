namespace Gast.Domain.Stats
{
    /// <summary>
    /// Definition data for a character stat (e.g., Health, Stamina).
    /// </summary>
    public interface IStatDefinition
    {
        /// <summary>
        /// Unique identifier for this stat type.
        /// </summary>
        StatId Id { get; }

        /// <summary>
        /// Display name for this stat.
        /// </summary>
        string DisplayName { get; }
    }
}
