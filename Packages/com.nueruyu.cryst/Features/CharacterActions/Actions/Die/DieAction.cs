using System;
using Cryst.Domain.Characters.Commands;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Die
{
    public class DieAction : ICharacterExecutableAction<DieCommand>
    {
        readonly CharacterBody body;
        readonly CharacterController controller;
        readonly CharacterAnimator animator;

        public Type CommandType => typeof(DieCommand);
        public int Priority => 99;

        public DieAction(CharacterBody body, CharacterAnimator animator)
        {
            this.body = body;
            controller = body.GetComponent<CharacterController>();
            this.animator = animator;
        }

        public bool CanExecute() => true;

        public void Execute(in DieCommand command)
        {
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

            body.IsInputMovementEnabled = true;
        }
    }
}