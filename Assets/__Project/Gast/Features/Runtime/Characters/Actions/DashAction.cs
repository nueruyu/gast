using System;
using UnityEngine;
using Gast.Features.Characters.Actions.Commands;

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
        readonly DashActionSettings settings;

        bool isActive;
        float startTime;
        float lastDashTime = float.NegativeInfinity;
        Vector3 dashDirection;

        public Type CommandType => typeof(DashCommand);
        public int Priority => 10;
        public bool IsActive => isActive;

        public DashAction(
            CharacterContext character,
            DashActionSettings settings)
        {
            this.body = character.Body;
            this.animator = character.Animator;
            this.settings = settings;
        }

        public bool CanExecute()
        {
            return !isActive && Time.time >= lastDashTime + settings.Cooldown;
        }

        public void Execute()
        {
            isActive = true;
            startTime = Time.time;
            lastDashTime = startTime;

            body.IsInputMovementEnabled = false;

            dashDirection = body.Forward;

            if (animator)
                animator.PlayDash();
        }

        public void OnUpdate()
        {
            float elapsed = Time.time - startTime;
            float progress = elapsed / settings.Duration;

            if (progress >= 1.0f)
            {
                isActive = false;
                return;
            }

            float speedEval = settings.SpeedCurve.Evaluate(progress);
            body.SetForcedVelocity(dashDirection * (settings.MaxSpeed * speedEval));
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
