using Gast.Features.Characters;
using System;
using UnityEngine;

namespace Cryst.Modules.CharacterActions
{
    public class GuardAction : ICharacterExecutableAction<GuardCommand>
    {
        readonly CharacterContext context;
        readonly GuardActionSettings settings;
        readonly CharacterAnimator animator;
        readonly CharacterMovement movement;

        public Type CommandType => typeof(GuardCommand);
        public int Priority => 2;

        public GuardAction(CharacterContext context, GuardActionSettings settings)
        {
            this.context = context;
            this.settings = settings;
            animator = context.Resolve<CharacterAnimator>();
            movement = context.Resolve<CharacterMovement>();
        }

        public bool CanExecute() => true;

        public void Execute(in GuardCommand command)
        {
            animator?.SetGuard(true);
        }

        public bool OnUpdate()
        {
            return true;
        }

        public void Move(Vector3 direction)
        {
            var speed = context.TypeDefinition.WalkSpeed * settings.MoveSpeedPenalty;
            movement.Move(direction, speed, settings.LookDirectionSpeed);
        }

        public void OnEnd()
        {
            animator?.SetGuard(false);
        }
    }
}