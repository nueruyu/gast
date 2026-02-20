using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using System.Threading;
using UnityEngine;

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
        /// Identifier for this character's type definition.
        /// </summary>
        CharacterTypeId TypeId { get; }

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

        /// <summary>
        /// Wallet managing the character's currency.
        /// </summary>
        Wallet Wallet { get; }

        /// <summary>
        /// Inventory managing the character's items.
        /// </summary>
        Inventory Inventory { get; }

        /// <summary>
        /// Attach a brain to this character, detaching any existing brain first.
        /// </summary>
        void AttachBrain(ICharacterBrain newBrain);

        /// <summary>
        /// Detach the current brain from this character.
        /// </summary>
        void DetachBrain();

        void Destroy();

        /// <summary>
        /// Gets a specific facet of the character.
        /// </summary>
        /// <typeparam name="T">The type of the facet to get, which must implement ICharacterFacet.</typeparam>
        /// <returns>The requested facet instance, or null if not available.</returns>
        T As<T>() where T : class, ICharacterFacet;

        /// <summary>
        /// Resolves a service registered in this character's context container.
        /// </summary>
        T Resolve<T>() where T : class;

        bool TryResolve<T>(out T module) where T : class;
    }
}