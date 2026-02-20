using Gast.Features.Characters;
using System;
using UnityEngine;

namespace Cryst.Modules.CharacterActions
{
    public class JumpAction : ICharacterExecutableAction<JumpCommand>
    {
        readonly CharacterContext context;
        readonly JumpActionSettings settings;
        readonly CharacterBody body;
        readonly CharacterMovement movement;

        public Type CommandType => typeof(JumpCommand);
        public int Priority => 3;

        public JumpAction(
            CharacterContext context,
            JumpActionSettings settings,
            CharacterBody body,
            CharacterMovement movement)
        {
            this.context = context;
            this.settings = settings;
            this.body = body;
            this.movement = movement;
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
            var speed = context.TypeDefinition.WalkSpeed;
            movement.Move(direction, speed, settings.LookDirectionSpeed);
        }

        public void OnEnd()
        {
        }
    }
}