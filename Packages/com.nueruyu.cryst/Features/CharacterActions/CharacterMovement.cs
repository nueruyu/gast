using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions
{
    /// <summary>
    /// Encapsulates character movement logic, including physical motion and animation updates.
    /// This class is registered in the CharacterContext and used by various character actions.
    /// </summary>
    public class CharacterMovement
    {
        readonly CharacterAnimator animator;
        readonly CharacterBody body;
        readonly ICharacterTypeDefinition typeDefinition;

        public CharacterMovement(CharacterAnimator animator, CharacterBody body, ICharacterTypeDefinition typeDefinition)
        {
            this.animator = animator;
            this.body = body;
            this.typeDefinition = typeDefinition;
        }

        /// <summary>
        /// Moves the character based on the given direction and speed, and updates animations.
        /// </summary>
        /// <param name="direction">The desired movement direction.</param>
        /// <param name="targetSpeed">The absolute speed for the movement.</param>
        /// <param name="lookSpeed">The speed of rotation towards the movement direction.</param>
        public void Move(Vector3 direction, float targetSpeed, float lookSpeed = 10f)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                body.SetInputVelocity(direction * targetSpeed);
                Look(direction, lookSpeed);
            }
            else
            {
                body.SetInputVelocity(Vector3.zero);
            }

            var movementSettings = typeDefinition.GetSettings<CharacterMovementSettings>();
            var normalizedSpeed = targetSpeed / movementSettings.SprintSpeed;
            if (animator)
            {
                animator.SetMoveSpeed(normalizedSpeed);
            }
        }

        /// <summary>
        /// Stops all character movement and resets the animation speed.
        /// </summary>
        public void Stop()
        {
            body.SetInputVelocity(Vector3.zero);
            if (animator)
            {
                animator.SetMoveSpeed(0);
            }
        }

        /// <summary>
        /// Rotates the character to face a specific direction.
        /// </summary>
        /// <param name="direction">The direction to look at.</param>
        /// <param name="speed">The rotation speed.</param>
        public void Look(Vector3 direction, float speed)
        {
            body.SetLookDirection(direction, speed);
        }
    }
}