using Cryst.Domain.Characters;
using Gast.Domain.Characters;
using System;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Default
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
            float targetSpeed;
            if (direction.sqrMagnitude > 0.01f)
            {
                targetSpeed = stateStore.IsSprinting ? typeDefinition.SprintSpeed : typeDefinition.WalkSpeed;
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