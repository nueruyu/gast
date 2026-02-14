using Gast.Features.Characters;
using GastGame.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    public class DieAction : ICharacterExecutableAction<DieCommand>
    {
        readonly CharacterContext context;
        readonly CharacterController controller;
        readonly CharacterAnimator animator;

        public Type CommandType => typeof(DieCommand);
        public int Priority => 99;

        public DieAction(CharacterContext context)
        {
            this.context = context;
            controller = context.Body.GetComponent<CharacterController>();
            animator = context.Resolve<CharacterAnimator>();
        }

        public bool CanExecute() => true;

        public void Execute(in DieCommand command)
        {
            var body = context.Body;
            body.IsInputMovementEnabled = false;
            body.SetForcedVelocity(Vector3.zero);

            if (controller != null)
                controller.enabled = false;

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
            if (animator)
                animator.SetDead(false);

            context.Body.IsInputMovementEnabled = true;
        }
    }
}
