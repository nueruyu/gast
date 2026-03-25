using Cryst.Domain.Characters;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Default
{
    public class DefaultAction : ICharacterAction
    {
        readonly CharacterActionStateStore stateStore;
        readonly CharacterMovement movement;
        readonly CharacterMovementSettings movementSettings;

        public int Priority => 0;

        public DefaultAction(
            CharacterActionStateStore stateStore,
            CharacterMovement movement,
            CharacterMovementSettings movementSettings)
        {
            this.stateStore = stateStore;
            this.movement = movement;
            this.movementSettings = movementSettings;
        }

        public bool OnUpdate()
        {
            return true;
        }

        public void Move(Vector3 direction)
        {
            float targetSpeed;
            if (direction.sqrMagnitude > 0.01f)
            {
                targetSpeed = stateStore.IsSprinting ? movementSettings.SprintSpeed : movementSettings.WalkSpeed;
            }
            else
            {
                targetSpeed = 0f;
            }

            movement.Move(direction, targetSpeed);
        }

        public void OnEnd()
        {
            movement.Stop();
        }
    }
}