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

    /// <summary>
    /// Generic factory interface for creating character instances with strongly-typed parameters.
    /// Provides a default implementation of <see cref="ICharacterFactory.Create"/> that validates
    /// the parameter type before delegating to the typed overload.
    /// </summary>
    public interface ICharacterFactory<TParameters> : ICharacterFactory
        where TParameters : ICharacterCreationParameters
    {
        Type ICharacterFactory.ParametersType => typeof(TParameters);

        ICharacter ICharacterFactory.Create(Vector3 position, Quaternion rotation, ICharacterCreationParameters parameters)
        {
            if (parameters is not TParameters typedParameters)
            {
                throw new ArgumentException($"Invalid parameter type. Expected {typeof(TParameters).Name}.", nameof(parameters));
            }

            return Create(position, rotation, typedParameters);
        }

        /// <summary>
        /// Create a new character instance with strongly-typed parameters.
        /// </summary>
        /// <param name="position">World position for the character.</param>
        /// <param name="rotation">World rotation for the character.</param>
        /// <param name="parameters">Typed parameters required for character creation.</param>
        /// <returns>The created character instance.</returns>
        ICharacter Create(Vector3 position, Quaternion rotation, TParameters parameters);
    }
}
