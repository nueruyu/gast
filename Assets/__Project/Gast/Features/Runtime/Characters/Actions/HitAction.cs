using System;
using Gast.Domain.Combat;
using Gast.Features.Characters.Actions.Commands;
using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    /// <summary>
    /// Handles hit reaction: plays animation and applies knockback.
    /// High priority interrupts most other actions (except dash/invincibility actions).
    /// </summary>
    public class HitAction : ICharacterAction<HitCommand>
    {
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        readonly HitActionSettings settings;

        float startTime;

        Vector3 knockbackVelocity;

        public Type CommandType => typeof(HitCommand);

        // Priority 8: Higher than Attack(5), lower than Dash(10)
        // This allows dash to avoid hits, but interrupts attacks and movement
        public int Priority => 8;

        public HitAction(CharacterContext character, HitActionSettings settings)
        {
            this.body = character.Body;
            this.animator = character.Animator;
            this.settings = settings;
        }

        public bool CanExecute() => true; // Can always execute when hit

        public void Execute(in HitCommand command)
        {
            startTime = Time.time;

            knockbackVelocity = command.DamageInfo.KnockbackForce;

            // Play hit animation
            if (animator)
                animator.PlayHit();

            // Apply initial knockback velocity
            body.SetForcedVelocity(knockbackVelocity);

            // Disable input movement (stun effect)
            body.IsInputMovementEnabled = false;
        }

        public bool OnUpdate()
        {
            if (Time.time >= startTime + settings.Duration)
            {
                return false;
            }

            // Apply friction to knockback velocity
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * 5f);
            body.SetForcedVelocity(knockbackVelocity);
            return true;
        }

        public void Move(Vector3 direction, float speed)
        {
            // Ignore movement input during hit reaction
        }

        public void OnEnd()
        {
            body.IsInputMovementEnabled = true; // Re-enable input
            body.SetForcedVelocity(Vector3.zero); // Clear knockback
        }
    }
}
