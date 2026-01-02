using UnityEngine;

namespace DescrioGames.Domain.Characters
{
    /// <summary>
    /// Read-only interface for querying character physical state.
    /// Does not provide any operation methods - use ICharacter for operations.
    /// </summary>
    public interface ICharacterBody
    {
        /// <summary>
        /// The character's current world position.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// The character's current world rotation.
        /// </summary>
        Quaternion Rotation { get; }

        /// <summary>
        /// The character's forward direction.
        /// </summary>
        Vector3 Forward { get; }

        /// <summary>
        /// Whether the character is currently grounded.
        /// </summary>
        bool IsGrounded { get; }

        /// <summary>
        /// The character's current velocity (physics).
        /// </summary>
        Vector3 Velocity { get; }
    }
}
