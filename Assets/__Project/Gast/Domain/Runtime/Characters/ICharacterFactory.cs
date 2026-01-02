using UnityEngine;

namespace DescrioGames.Domain.Characters
{
    /// <summary>
    /// Factory interface for creating character instances.
    /// </summary>
    public interface ICharacterFactory
    {
        /// <summary>
        /// Create a new character instance from a character type ID.
        /// </summary>
        /// <param name="typeId">The character type identifier.</param>
        /// <param name="position">World position for the character.</param>
        /// <param name="rotation">World rotation for the character.</param>
        /// <param name="faction">Optional faction override. If null, uses definition's default faction.</param>
        /// <returns>The created character instance.</returns>
        ICharacter Create(CharacterTypeId typeId, Vector3 position, Quaternion rotation, Faction faction);
    }
}