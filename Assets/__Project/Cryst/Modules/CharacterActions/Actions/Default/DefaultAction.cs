using Gast.Features.Characters;
using System;
using UnityEngine;

namespace Cryst.Modules.CharacterActions.Default
{
    public class DefaultAction : ICharacterAction
    {
        readonly CharacterContext context;
        readonly CharacterActionStateStore stateStore;
        readonly CharacterMovement movement;

        public int Priority => 0;

        public DefaultAction(CharacterContext context)
        {
            this.context = context;
            stateStore = context.Resolve<CharacterActionStateStore>();
            movement = context.Resolve<CharacterMovement>();
        }

        public bool OnUpdate()
        {
            return true;
        }

        public void Move(Vector3 direction)
        {
            var typeDef = context.TypeDefinition;

            float targetSpeed;
            if (direction.sqrMagnitude > 0.01f)
            {
                targetSpeed = stateStore.IsSprinting ? typeDef.SprintSpeed : typeDef.WalkSpeed;
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