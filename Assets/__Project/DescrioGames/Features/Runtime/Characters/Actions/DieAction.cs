using UnityEngine;

namespace DescrioGames.Features.Characters.Actions
{
    /// <summary>
    /// Death action with highest priority (99).
    /// Disables movement and triggers death animation.
    /// </summary>
    public class DieAction : ICharacterAction
    {
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        bool isActive;

        public int Priority => 99; // Highest priority - interrupts everything
        public bool IsActive => isActive;

        public DieAction(CharacterContext character)
        {
            this.body = character.Body;
            this.animator = character.Animator;
        }

        public bool CanExecute() => true;

        public void Execute()
        {
            isActive = true;
            body.IsInputMovementEnabled = false;
            body.SetForcedVelocity(Vector3.zero);

            if (animator)
                animator.SetDead(true);
        }

        public void OnUpdate()
        {
        }

        public void Move(Vector3 direction, float speed)
        {
            // No movement allowed when dead
        }

        public void OnEnd()
        {
            isActive = false;

            if (animator)
                animator.SetDead(false);

            body.IsInputMovementEnabled = true;
        }
    }
}