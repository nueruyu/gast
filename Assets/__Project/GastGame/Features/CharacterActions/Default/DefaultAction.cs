using Gast.Features.Characters;
using GastGame.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions.Default
{
    public class DefaultAction : ICharacterAction
    {
        readonly CharacterActionContext context;

        public Type CommandType => null; // This action is not triggered by a command
        public int Priority => 0; // Lowest priority

        public DefaultAction(CharacterActionContext context)
        {
            this.context = context;
        }

        public bool CanExecute() => true;

        public bool OnUpdate()
        {
            // This action runs indefinitely until interrupted
            return true;
        }

        public void Move(Vector3 direction)
        {
            var body = context.CharacterContext.Body;
            var typeDef = context.CharacterContext.TypeDefinition;
            var animator = context.CharacterAnimator;
            var stateStore = context.StateStore;

            float targetSpeed;
            if (direction.sqrMagnitude > 0.01f)
            {
                targetSpeed = stateStore.IsSprinting ? typeDef.SprintSpeed : typeDef.WalkSpeed;
                body.SetInputVelocity(direction * targetSpeed);
                body.SetLookDirection(direction, 10f);
            }
            else
            {
                targetSpeed = 0f;
                body.SetInputVelocity(Vector3.zero);
            }

            var normalizedSpeed = targetSpeed / typeDef.SprintSpeed;
            animator.SetMoveSpeed(normalizedSpeed);
        }

        public void OnEnd()
        {
            // When interrupted, ensure move speed is reset in animator
            context.CharacterAnimator.SetMoveSpeed(0);
        }
    }
}
