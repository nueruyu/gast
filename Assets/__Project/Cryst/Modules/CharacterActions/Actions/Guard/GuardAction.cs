using Gast.Domain.Characters;
using Gast.Features.Characters;
using System;
using UnityEngine;

namespace Cryst.Modules.CharacterActions
{
    public class GuardAction : ICharacterExecutableAction<GuardCommand>
    {
        readonly GuardActionSettings settings;
        readonly CharacterAnimator animator;
        readonly CharacterMovement movement;
        readonly ICharacterTypeDefinition typeDefinition;

        public Type CommandType => typeof(GuardCommand);
        public int Priority => 2;

        public GuardAction(
            GuardActionSettings settings,
            CharacterAnimator animator,
            CharacterMovement movement,
            ICharacterTypeDefinition typeDefinition)
        {
            this.settings = settings;
            this.animator = animator;
            this.movement = movement;
            this.typeDefinition = typeDefinition;
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
            var speed = typeDefinition.WalkSpeed * settings.MoveSpeedPenalty;
            movement.Move(direction, speed, settings.LookDirectionSpeed);
        }

        public void OnEnd()
        {
            animator?.SetGuard(false);
        }
    }
}