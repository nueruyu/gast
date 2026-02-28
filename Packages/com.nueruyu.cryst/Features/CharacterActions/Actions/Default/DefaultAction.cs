using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using Gast.Unity.Infrastructure.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Default
{
    public class DefaultAction : ICharacterAction
    {
        readonly CharacterActionStateStore stateStore;
        readonly CharacterMovement movement;
        readonly ICharacterTypeDefinition typeDefinition;

        public int Priority => 0;

        public DefaultAction(
            CharacterActionStateStore stateStore,
            CharacterMovement movement,
            ICharacterTypeDefinition typeDefinition)
        {
            this.stateStore = stateStore;
            this.movement = movement;
            this.typeDefinition = typeDefinition;
        }

        public bool OnUpdate()
        {
            return true;
        }

        public void Move(Vector3 direction)
        {
            var movementSettings = typeDefinition.GetSettings<CharacterMovementSettings>();
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