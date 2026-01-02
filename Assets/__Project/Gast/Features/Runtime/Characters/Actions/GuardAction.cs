using UnityEngine;

namespace DescrioGames.Features.Characters.Actions
{
    /// <summary>
    /// Guard action that allows reduced-speed movement while guarding.
    /// Reads latest input from CharacterActionController and applies movement penalty.
    /// </summary>
    public class GuardAction : ICharacterAction
    {
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        readonly float moveSpeedPenalty = 0.5f;

        bool isActive;

        public int Priority => 2;
        public bool IsActive => isActive;

        public GuardAction(CharacterContext character)
        {
            this.body = character.Body;
            this.animator = character.Animator;
        }

        public bool CanExecute() => true;

        public void Execute()
        {
            isActive = true;
            animator?.SetGuard(true);
        }

        /// <summary>
        /// Manually stop guard (called when button is released).
        /// </summary>
        public void ManualStop()
        {
            isActive = false;
        }

        public void OnUpdate()
        {
            // OnUpdate does nothing - movement is handled via Move() method
        }

        public void Move(Vector3 direction, float speed)
        {
            // Apply reduced-speed movement while guarding
            if (direction.sqrMagnitude > 0.01f)
            {
                body.SetInputVelocity(direction * (speed * moveSpeedPenalty));
                body.SetLookDirection(direction, 5f); // Slower rotation while guarding
            }
        }

        public void OnEnd()
        {
            isActive = false;
            animator?.SetGuard(false);
        }
    }
}