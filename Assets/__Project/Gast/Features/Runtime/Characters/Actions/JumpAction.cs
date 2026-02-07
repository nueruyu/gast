using System;
using UnityEngine;
using Gast.Features.Characters.Actions.Commands;

namespace Gast.Features.Characters.Actions
{
    public class JumpAction : ICharacterAction<JumpCommand>
    {
        readonly CharacterBody body;
        readonly JumpActionSettings settings;

        public Type CommandType => typeof(JumpCommand);
        public int Priority => 3;

        public JumpAction(CharacterContext character, JumpActionSettings settings)
        {
            this.body = character.Body;
            this.settings = settings;
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
            // Jump is an instant action.
            return false;
        }

        public void Move(Vector3 direction, float speed)
        {
            // Allow air control by normal movement logic
            if (direction.sqrMagnitude > 0.01f)
            {
                body.SetInputVelocity(direction * speed);
                body.SetLookDirection(direction, 10f);
            }
        }

        public void OnEnd()
        {
        }
    }
}
