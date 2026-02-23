using Gast.Core.Observables;
using Gast.Domain.Interactions;
using System.Threading;

namespace Gast.Domain.Characters
{
    /// <summary>
    /// Interface representing a character entity in the game world.
    /// Provides both state queries (via Body) and operation methods.
    /// </summary>
    public interface ICharacter
    {
        /// <summary>
        /// Unique identifier for this character instance.
        /// </summary>
        CharacterId Id { get; }

        /// <summary>
        /// The character's faction affiliation.
        /// </summary>
        Faction Faction { get; }

        /// <summary>
        /// The character's type definition data.
        /// </summary>
        ICharacterTypeDefinition TypeDefinition { get; }

        /// <summary>
        /// Signal raised when this character is destroyed.
        /// </summary>
        ISignal<ICharacter> Destroyed { get; }

        CancellationToken CancellationToken { get; }

        /// <summary>
        /// Controller for managing the character's actions.
        /// </summary>
        ICharacterActionController ActionController { get; }

        void Destroy();

        /// <summary>
        /// Tries to get a specific facet of the character.
        /// </summary>
        /// <typeparam name="T">The type of the facet to get, which must implement ICharacterFacet.</typeparam>
        /// <param name="facet">The output facet instance if found, otherwise null.</param>
        /// <returns>True if the facet was found, otherwise false.</returns>
        bool Is<T>(out T facet) where T : class, ICharacterFacet;

        /// <summary>
        /// Resolves a service registered in this character's context container.
        /// </summary>
        T Resolve<T>() where T : class;

        bool TryResolve<T>(out T module) where T : class;
    }
}