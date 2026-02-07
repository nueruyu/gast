using Gast.Features.Characters.Actions.Commands;
using System;
using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    public class DieAction : ICharacterAction<DieCommand>
    {
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        readonly CharacterController controller;

        public Type CommandType => typeof(DieCommand);
        public int Priority => 99;

        public DieAction(CharacterContext character)
        {
            this.body = character.Body;
            this.animator = character.Animator;
            this.controller = character.Body.GetComponent<CharacterController>();
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

        public void Move(Vector3 direction, float speed)
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
