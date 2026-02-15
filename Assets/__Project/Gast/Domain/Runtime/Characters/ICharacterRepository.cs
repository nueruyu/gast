using System.Collections.Generic;
using Gast.Core.Observables;

namespace Gast.Domain.Characters
{
    /// <summary>
    /// Repository for managing runtime character instances.
    /// </summary>
    public interface ICharacterRepository
    {
        /// <summary>
        /// Register a character instance to the repository.
        /// </summary>
        void Register(ICharacter character);

        /// <summary>
        /// Unregister a character instance from the repository.
        /// </summary>
        void Unregister(CharacterId id);

        /// <summary>
        /// Get a character by its unique identifier.
        /// </summary>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no character with the given ID exists.
        /// </exception>
        ICharacter Get(CharacterId id);

        /// <summary>
        /// Get all registered characters.
        /// </summary>
        IEnumerable<ICharacter> GetAll();

        /// <summary>
        /// Signal emitted when a new character is registered.
        /// </summary>
        ISignal<ICharacter> Registered { get; }

        /// <summary>
        /// Signal emitted when a character is unregistered.
        /// </summary>
        ISignal<ICharacter> Unregistered { get; }
    }
}