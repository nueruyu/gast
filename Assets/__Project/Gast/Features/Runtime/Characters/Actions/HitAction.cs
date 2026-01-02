using DescrioGames.Domain.Combat;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace DescrioGames.Features.Characters.Actions
{
    /// <summary>
    /// Handles hit reaction: plays animation and applies knockback.
    /// High priority interrupts most other actions (except dash/invincibility actions).
    /// </summary>
    public class HitAction : ICharacterAction
    {
        readonly CharacterBody body;
        readonly CharacterAnimator animator;

        bool isActive;
        float startTime;
        float duration = 0.5f; // Duration should match animation length

        Vector3 knockbackVelocity;

        // Priority 8: Higher than Attack(5), lower than Dash(10)
        // This allows dash to avoid hits, but interrupts attacks and movement
        public int Priority => 8;

        public bool IsActive => isActive;

        public HitAction(CharacterContext character)
        {
            this.body = character.Body;
            this.animator = character.Animator;
        }

        /// <summary>
        /// Setup the hit reaction with damage information.
        /// Call this before executing the action.
        /// </summary>
        public void Setup(DamageInfo info)
        {
            knockbackVelocity = info.KnockbackForce;
        }

        public bool CanExecute() => true; // Can always execute when hit

        public void Execute()
        {
            isActive = true;
            startTime = Time.time;

            // Play hit animation
            if (animator)
                animator.PlayHit();

            // Apply initial knockback velocity
            body.SetForcedVelocity(knockbackVelocity);

            // Disable input movement (stun effect)
            body.IsInputMovementEnabled = false;
        }

        public void OnUpdate()
        {
            if (Time.time >= startTime + duration)
            {
                isActive = false;
                return;
            }

            // Apply friction to knockback velocity
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);
            body.SetForcedVelocity(knockbackVelocity);
        }

        public void Move(Vector3 direction, float speed)
        {
            // Ignore movement input during hit reaction
        }

        public void OnEnd()
        {
            isActive = false;
            body.IsInputMovementEnabled = true; // Re-enable input
            body.SetForcedVelocity(Vector3.zero); // Clear knockback
        }
    }
}