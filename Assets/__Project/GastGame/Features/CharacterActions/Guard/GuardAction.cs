using Gast.Features.Characters;
using GastGame.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    public class GuardAction : ICharacterExecutableAction<GuardCommand>
    {
        readonly CharacterContext context;
        readonly GuardActionSettings settings;
        readonly CharacterAnimator animator;

        public Type CommandType => typeof(GuardCommand);
        public int Priority => 2;

        public GuardAction(CharacterContext context, GuardActionSettings settings)
        {
            this.context = context;
            this.settings = settings;
            animator = context.Resolve<CharacterAnimator>();
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
            if (direction.sqrMagnitude > 0.01f)
            {
                var speed = context.TypeDefinition.WalkSpeed * settings.MoveSpeedPenalty;
                context.Body.SetInputVelocity(direction * speed);
                context.Body.SetLookDirection(direction, settings.LookDirectionSpeed);
            }
        }

        public void OnEnd()
        {
            animator?.SetGuard(false);
        }
    }
}
