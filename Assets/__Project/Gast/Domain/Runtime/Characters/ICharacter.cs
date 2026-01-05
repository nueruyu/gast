using Gast.Core.Observables;
using Gast.Domain.AI;
using Gast.Domain.Combat;
using Gast.Domain.Economy;
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
        /// Character's health and status information.
        /// </summary>
        CharacterStatus Status { get; }

        /// <summary>
        /// Read-only access to the character's physical state.
        /// </summary>
        ICharacterBody Body { get; }

        /// <summary>
        /// Whether this character is currently alive.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Whether the character is ready to attack (e.g. not in cooldown).
        /// </summary>
        bool CanAttack { get; }

        /// <summary>
        /// Whether the character is currently guarding.
        /// </summary>
        bool IsGuarding { get; }

        /// <summary>
        /// Whether the character is currently dashing.
        /// </summary>
        bool IsDashing { get; }

        /// <summary>
        /// Signal raised when this character is destroyed.
        /// </summary>
        ISignal<ICharacter> Destroyed { get; }

        /// <summary>
        /// Wallet managing the character's currency.
        /// </summary>
        Wallet Wallet { get; }

        /// <summary>
        /// Inventory managing the character's items.
        /// </summary>
        Inventory Inventory { get; }

        IVisionSensor VisionSensor { get; }

        INavigationProvider NavigationProvider { get; }

        // === Operation Methods ===

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

        /// <summary>
        /// Make the character jump if grounded.
        /// </summary>
        void Jump();

        /// <summary>
        /// Execute an attack action.
        /// </summary>
        void Attack();

        /// <summary>
        /// Perform a dash action in the specified direction.
        /// </summary>
        void Dash(Vector3 direction);

        /// <summary>
        /// Set the guard state.
        /// </summary>
        void SetGuard(bool active);

        /// <summary>
        /// Apply damage and hit reaction to the character.
        /// </summary>
        void TakeDamage(DamageInfo info);

        /// <summary>
        /// Get the currently attached brain.
        /// </summary>
        ICharacterBrain GetBrain();
    }
}