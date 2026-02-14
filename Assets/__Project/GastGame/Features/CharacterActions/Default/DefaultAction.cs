using Gast.Features.Characters;
using GastGame.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions.Default
{
    public class DefaultAction : ICharacterAction
    {
        readonly CharacterContext context;
        readonly CharacterAnimator animator;
        readonly CharacterActionStateStore stateStore;

        public int Priority => 0;

        public DefaultAction(CharacterContext context)
        {
            this.context = context;
            animator = context.Resolve<CharacterAnimator>();
            stateStore = context.Resolve<CharacterActionStateStore>();
        }

        public bool OnUpdate()
        {
            return true;
        }

        public void Move(Vector3 direction)
        {
            var body = context.Body;
            var typeDef = context.TypeDefinition;

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
            if (animator)
                animator.SetMoveSpeed(normalizedSpeed);
        }

        public void OnEnd()
        {
            if (animator)
                animator.SetMoveSpeed(0);
        }
    }
}