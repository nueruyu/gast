using Gast.Features.Characters;
using GastGame.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    public class DieAction : ICharacterAction<DieCommand>
    {
        readonly CharacterActionContext context;
        readonly CharacterController controller;

        public Type CommandType => typeof(DieCommand);
        public int Priority => 99;

        public DieAction(CharacterActionContext context)
        {
            this.context = context;
            controller = context.CharacterContext.Body.GetComponent<CharacterController>();
        }

        public bool CanExecute() => true;

        public void Execute(in DieCommand command)
        {
            var body = context.CharacterContext.Body;
            body.IsInputMovementEnabled = false;
            body.SetForcedVelocity(Vector3.zero);

            if (controller != null)
                controller.enabled = false;

            var animator = context.CharacterAnimator;
            if (animator)
                animator.SetDead(true);
        }

        public bool OnUpdate()
        {
            return true;
        }

        public void Move(Vector3 direction)
        {
        }

        public void OnEnd()
        {
            var animator = context.CharacterAnimator;
            if (animator)
                animator.SetDead(false);

            context.CharacterContext.Body.IsInputMovementEnabled = true;
        }
    }
}
