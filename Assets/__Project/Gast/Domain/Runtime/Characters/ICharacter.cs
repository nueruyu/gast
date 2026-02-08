using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Combat;
using Gast.Domain.Economy;
using Gast.Domain.Interactions;
using Gast.Domain.Sensors;
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
        /// Character's generic status container.
        /// </summary>
        CharacterStatus Status { get; }

        /// <summary>
        /// The character's faction affiliation.
        /// </summary>
        Faction Faction { get; }

        /// <summary>
        /// Read-only access to the character's physical state.
        /// </summary>
        ICharacterBody Body { get; }

        /// <summary>
        /// Signal raised when this character is destroyed.
        /// </summary>
        ISignal<ICharacter> Destroyed { get; }

        ICharacterActionController ActionController { get; }

        /// <summary>
        /// Wallet managing the character's currency.
        /// </summary>
        Wallet Wallet { get; }

        /// <summary>
        /// Inventory managing the character's items.
        /// </summary>
        Inventory Inventory { get; }

        IVisionSensor VisionSensor { get; }

        IInteractionSensor InteractionSensor { get; }

        INavigationProvider NavigationProvider { get; }

        /// <summary>
        /// Attach a brain to this character, detaching any existing brain first.
        /// </summary>
        void AttachBrain(ICharacterBrain newBrain);

        /// <summary>
        /// Detach the current brain from this character.
        /// </summary>
        void DetachBrain();

        /// <summary>
        /// Move the character in the specified direction (0-1 normalized).
        /// </summary>
        void Move(Vector3 direction);

        /// <summary>
        /// Set whether the character is sprinting.
        /// </summary>
        void SetSprint(bool isSprinting);
    }
}