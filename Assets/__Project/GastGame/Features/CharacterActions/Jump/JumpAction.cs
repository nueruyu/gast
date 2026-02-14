using Gast.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    public class JumpAction : ICharacterExecutableAction<JumpCommand>
    {
        readonly CharacterContext context;
        readonly JumpActionSettings settings;

        public Type CommandType => typeof(JumpCommand);
        public int Priority => 3;

        public JumpAction(CharacterContext context, JumpActionSettings settings)
        {
            this.context = context;
            this.settings = settings;
        }

        public bool CanExecute()
        {
            return context.Body.IsGrounded;
        }

        public void Execute(in JumpCommand command)
        {
            context.Body.ApplyJump(settings.Force);
        }

        public bool OnUpdate()
        {
            return false;
        }

        public void Move(Vector3 direction)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                var speed = context.TypeDefinition.WalkSpeed;
                context.Body.SetInputVelocity(direction * speed);
                context.Body.SetLookDirection(direction, settings.LookDirectionSpeed);
            }
        }

        public void OnEnd()
        {
        }
    }
}
