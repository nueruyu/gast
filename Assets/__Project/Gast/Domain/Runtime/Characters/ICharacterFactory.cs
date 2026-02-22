using System;
using UnityEngine;

namespace Gast.Domain.Characters
{
    /// <summary>
    /// Factory interface for creating character instances.
    /// </summary>
    public interface ICharacterFactory
    {
        /// <summary>
        /// The type of parameters this factory can handle.
        /// </summary>
        Type ParametersType { get; }

        /// <summary>
        /// Create a new character instance.
        /// </summary>
        /// <param name="position">World position for the character.</param>
        /// <param name="rotation">World rotation for the character.</param>
        /// <param name="parameters">Parameters required for character creation.</param>
        /// <returns>The created character instance.</returns>
        ICharacter Create(Vector3 position, Quaternion rotation, ICharacterCreationParameters parameters);
    }
}
