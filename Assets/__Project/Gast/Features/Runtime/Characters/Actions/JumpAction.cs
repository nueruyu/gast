using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    public class JumpAction : ICharacterAction
    {
        readonly CharacterBody body;
        readonly JumpActionSettings settings;
        bool isActive;

        public int Priority => 3;
        public bool IsActive => isActive;

        public JumpAction(CharacterContext character, JumpActionSettings settings)
        {
            this.body = character.Body;
            this.settings = settings;
        }

        public bool CanExecute()
        {
            return !isActive && body.IsGrounded;
        }

        public void Execute()
        {
            isActive = true;
            body.ApplyJump(settings.Force);
        }

        public void OnUpdate()
        {
            // Jump is an instant action.
            isActive = false;
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
            isActive = false;
        }
    }
}
