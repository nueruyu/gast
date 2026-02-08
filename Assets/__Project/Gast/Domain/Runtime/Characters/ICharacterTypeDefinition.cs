using System.Collections.Generic;
using Gast.Domain.Loot;
using Gast.Domain.Stats;

namespace Gast.Domain.Characters
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
        /// Movement speed for characters of this type.
        /// </summary>
        float WalkSpeed { get; }

        /// <summary>
        /// Sprint speed for characters of this type.
        /// </summary>
        float SprintSpeed { get; }

        ILootTable LootTable { get; }
    }
}