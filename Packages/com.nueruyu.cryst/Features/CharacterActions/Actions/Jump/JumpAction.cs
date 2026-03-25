using System;
using Cryst.Domain.Characters.Commands;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Jump
{
    public class JumpAction : ICharacterExecutableAction<JumpCommand>
    {
        readonly JumpActionSettings settings;
        readonly CharacterBody body;
        readonly CharacterMovement movement;
        readonly CharacterMovementSettings movementSettings;

        public Type CommandType => typeof(JumpCommand);
        public int Priority => 3;

        public JumpAction(
            JumpActionSettings settings,
            CharacterBody body,
            CharacterMovement movement,
            CharacterMovementSettings movementSettings)
        {
            this.settings = settings;
            this.body = body;
            this.movement = movement;
            this.movementSettings = movementSettings;
        }

        public bool CanExecute()
        {
            return body.IsGrounded;
        }

        public void Execute(in JumpCommand command)
        {
            body.ApplyJump(settings.Force);
        }

        public bool OnUpdate()
        {
            return false;
        }

        public void Move(Vector3 direction)
        {
            var speed = movementSettings.WalkSpeed;
            movement.Move(direction, speed, settings.LookDirectionSpeed);
        }

        public void OnEnd()
        {
        }
    }
}