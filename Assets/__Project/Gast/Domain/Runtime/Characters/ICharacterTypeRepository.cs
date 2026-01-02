namespace Gast.Domain.Characters
{
    /// <summary>
    /// Repository for accessing character type definitions.
    /// </summary>
    public interface ICharacterTypeRepository
    {
        /// <summary>
        /// Get a character type definition by its identifier.
        /// </summary>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Thrown when no definition with the given ID exists.
        /// </exception>
        ICharacterTypeDefinition Get(CharacterTypeId id);
    }
}
