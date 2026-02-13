using Gast.Features.Characters;
using GastGame.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    public class JumpAction : ICharacterAction<JumpCommand>
    {
        readonly CharacterActionContext context;
        readonly JumpActionSettings settings;

        public Type CommandType => typeof(JumpCommand);
        public int Priority => 3;

        public JumpAction(CharacterActionContext context, JumpActionSettings settings)
        {
            this.context = context;
            this.settings = settings;
        }

        public bool CanExecute()
        {
            return context.CharacterContext.Body.IsGrounded;
        }

        public void Execute(in JumpCommand command)
        {
            context.CharacterContext.Body.ApplyJump(settings.Force);
        }

        public bool OnUpdate()
        {
            // Jump is an instant action.
            return false;
        }

        public void Move(Vector3 direction)
        {
            // Allow air control by normal movement logic
            if (direction.sqrMagnitude > 0.01f)
            {
                var speed = context.CharacterContext.TypeDefinition.WalkSpeed;
                context.CharacterContext.Body.SetInputVelocity(direction * speed);
                context.CharacterContext.Body.SetLookDirection(direction, settings.LookDirectionSpeed);
            }
        }

        public void OnEnd()
        {
        }
    }
}
