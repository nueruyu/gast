using DescrioGames.Domain.Loot;

namespace DescrioGames.Domain.Characters
{
    /// <summary>
    /// Definition data for a character type (e.g., soldier, villager).
    /// </summary>
    public interface ICharacterTypeDefinition
    {
        /// <summary>
        /// Unique identifier for this character type.
        /// </summary>
        CharacterTypeId TypeId { get; }

        /// <summary>
        /// Display name for this character type.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Maximum health for characters of this type.
        /// </summary>
        float MaxHealth { get; }

        /// <summary>
        /// Movement speed for characters of this type.
        /// </summary>
        float WalkSpeed { get; }

        /// <summary>
        /// Sprint speed for characters of this type.
        /// </summary>
        float SprintSpeed { get; }

        /// <summary>
        /// Dash force applied during dodge action.
        /// </summary>
        float DashForce { get; }

        /// <summary>
        /// Duration of the dash state in seconds.
        /// </summary>
        float DashDuration { get; }

        /// <summary>
        /// Cooldown time before dash can be used again in seconds.
        /// </summary>
        float DashCooldown { get; }

        /// <summary>
        /// Whether this character type can guard.
        /// </summary>
        bool CanGuard { get; }

        ILootTable LootTable { get; }

        /// <summary>
        /// Jump force applied when the character jumps.
        /// </summary>
        float JumpForce { get; }
    }
}