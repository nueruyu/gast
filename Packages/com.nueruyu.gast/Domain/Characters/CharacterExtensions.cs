using System;

namespace Gast.Domain.Characters
{
    public static class CharacterExtensions
    {
        /// <summary>
        /// Gets a specific facet of the character, throwing an exception if not found.
        /// </summary>
        /// <typeparam name="T">The type of the facet to get.</typeparam>
        /// <param name="character">The character instance.</param>
        /// <returns>The requested facet instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the facet is not found on the character.</exception>
        public static T As<T>(this ICharacter character) where T : class, ICharacterFacet
        {
            if (character.Is(out T facet))
            {
                return facet;
            }
            throw new InvalidOperationException($"Character with ID '{character.Id}' does not have a facet of type '{typeof(T).Name}'.");
        }
    }
}