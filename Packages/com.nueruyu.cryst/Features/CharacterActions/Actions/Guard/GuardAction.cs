using System;
using Cryst.Domain.Characters.Commands;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Guard
{
    public class GuardAction : ICharacterExecutableAction<GuardCommand>
    {
        readonly GuardActionSettings settings;
        readonly CharacterAnimator animator;
        readonly CharacterMovement movement;
        readonly CharacterMovementSettings movementSettings;

        public Type CommandType => typeof(GuardCommand);
        public int Priority => 2;

        public GuardAction(
            GuardActionSettings settings,
            CharacterAnimator animator,
            CharacterMovement movement,
            CharacterMovementSettings movementSettings)
        {
            this.settings = settings;
            this.animator = animator;
            this.movement = movement;
            this.movementSettings = movementSettings;
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
            var speed = movementSettings.WalkSpeed * settings.MoveSpeedPenalty;
            movement.Move(direction, speed, settings.LookDirectionSpeed);
        }

        public void OnEnd()
        {
            animator?.SetGuard(false);
        }
    }
}