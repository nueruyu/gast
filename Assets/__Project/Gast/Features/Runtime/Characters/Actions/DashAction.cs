using UnityEngine;

namespace Gast.Features.Characters.Actions
{
    /// <summary>
    /// Dash action with curve-driven movement.
    /// Disables input movement and applies forced velocity based on animation curve.
    /// </summary>
    public class DashAction : ICharacterAction
    {
        readonly CharacterBody body;
        readonly CharacterAnimator animator;

        readonly float duration;
        readonly float cooldown;
        readonly float maxSpeed;
        readonly AnimationCurve speedCurve;

        bool isActive;
        float startTime;
        float lastDashTime = float.NegativeInfinity;
        Vector3 dashDirection;

        public int Priority => 10;
        public bool IsActive => isActive;

        public DashAction(
            CharacterContext character,
            float duration,
            float cooldown,
            float maxSpeed,
            AnimationCurve speedCurve)
        {
            this.body = character.Body;
            this.animator = character.Animator;
            this.duration = duration;
            this.cooldown = cooldown;
            this.maxSpeed = maxSpeed;
            this.speedCurve = speedCurve;
        }

        public bool CanExecute()
        {
            return !isActive && Time.time >= lastDashTime + cooldown;
        }

        public void Execute()
        {
            isActive = true;
            startTime = Time.time;
            lastDashTime = startTime;

            // Disable input movement
            body.IsInputMovementEnabled = false;

            // Determine dash direction (current facing direction)
            dashDirection = body.Forward;

            // Play animation
            if (animator)
                animator.PlayDash();
        }

        public void OnUpdate()
        {
            float elapsed = Time.time - startTime;
            float progress = elapsed / duration;

            if (progress >= 1.0f)
            {
                isActive = false;
                return;
            }

            // Apply curve-driven velocity
            float speedEval = speedCurve.Evaluate(progress);
            body.SetForcedVelocity(dashDirection * (maxSpeed * speedEval));
            body.SetLookDirection(dashDirection, 100f);
        }

        public void Move(Vector3 direction, float speed)
        {
            // Dash controls movement completely - ignore input
        }

        public void OnEnd()
        {
            isActive = false;
            body.IsInputMovementEnabled = true;
        }
    }
}