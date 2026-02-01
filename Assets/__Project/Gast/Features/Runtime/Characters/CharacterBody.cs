using UnityEngine;
using Gast.Domain.Characters;

namespace Gast.Features.Characters
{
    /// <summary>
    /// Physical body implementation for characters using Unity's CharacterController.
    /// Implements ICharacterBody for read-only state access.
    /// Provides public operation methods for Character class to use.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CharacterBody : MonoBehaviour, ICharacterBody
    {
        [Header("Physics")]
        [SerializeField]
        float gravity = -15f;

        CharacterController controller;
        Vector3 verticalVelocity;
        Vector3 currentInputVelocity;
        Vector3 forcedVelocity;

        public bool IsInputMovementEnabled { get; set; } = true;

        // ICharacterBody implementation (read-only)
        public bool IsGrounded => controller.isGrounded;

        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public Vector3 Forward => transform.forward;
        public Vector3 Velocity => controller.velocity;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            ApplyPhysics();
        }

        // === Operation Methods (Public, not in ICharacterBody) ===

        /// <summary>
        /// Set the input velocity for this frame.
        /// Only applied if IsInputMovementEnabled is true.
        /// </summary>
        public void SetInputVelocity(Vector3 velocity)
        {
            if (IsInputMovementEnabled)
            {
                currentInputVelocity = velocity;
            }
        }

        /// <summary>
        /// Set forced velocity from actions (dash, knockback, etc.).
        /// This bypasses input control and is always applied.
        /// </summary>
        public void SetForcedVelocity(Vector3 velocity)
        {
            forcedVelocity = velocity;
        }

        /// <summary>
        /// Apply an upward jump force.
        /// </summary>
        public void ApplyJump(float force)
        {
            if (controller.isGrounded)
            {
                verticalVelocity.y = force;
            }
        }

        /// <summary>
        /// Rotate the character to face the specified direction.
        /// </summary>
        public void SetLookDirection(Vector3 direction, float speed)
        {
            if (direction.sqrMagnitude < 0.01f)
                return;

            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }

        /// <summary>
        /// Internal physics update - applies gravity and executes movement.
        /// </summary>
        void ApplyPhysics()
        {
            if (!controller.enabled)
                return;

            if (controller.isGrounded && verticalVelocity.y < 0)
            {
                verticalVelocity.y = -2f; // Small downward force to keep grounded
            }

            verticalVelocity.y += gravity * Time.deltaTime;

            // Combine input velocity (if enabled) with forced velocity
            var moveVelocity = (IsInputMovementEnabled ? currentInputVelocity : Vector3.zero) + forcedVelocity;

            // Apply movement
            var finalMove = moveVelocity * Time.deltaTime;
            finalMove.y += verticalVelocity.y * Time.deltaTime;
            controller.Move(finalMove);

            // Reset velocities for next frame
            currentInputVelocity = Vector3.zero;
            forcedVelocity = Vector3.zero;
        }
    }
}