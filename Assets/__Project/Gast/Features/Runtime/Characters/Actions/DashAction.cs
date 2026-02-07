using System;
using UnityEngine;
using Gast.Features.Characters.Actions.Commands;

namespace Gast.Features.Characters.Actions
{
    /// <summary>
    /// Dash action with curve-driven movement.
    /// Disables input movement and applies forced velocity based on animation curve.
    /// </summary>
    public class DashAction : ICharacterAction<DashCommand>
    {
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        readonly DashActionSettings settings;

        float startTime;
        float lastDashTime = float.NegativeInfinity;
        Vector3 dashDirection;

        public Type CommandType => typeof(DashCommand);
        public int Priority => 10;

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
            return Time.time >= lastDashTime + settings.Cooldown;
        }

        public void Execute(in DashCommand command)
        {
            startTime = Time.time;
            lastDashTime = startTime;

            body.IsInputMovementEnabled = false;

            dashDirection = body.Forward;

            if (animator)
                animator.PlayDash();
        }

        public bool OnUpdate()
        {
            float elapsed = Time.time - startTime;
            float progress = elapsed / settings.Duration;

            if (progress >= 1.0f)
            {
                return false;
            }

            float speedEval = settings.SpeedCurve.Evaluate(progress);
            body.SetForcedVelocity(dashDirection * (settings.MaxSpeed * speedEval));
            body.SetLookDirection(dashDirection, 100f);

            return true;
        }

        public void Move(Vector3 direction, float speed)
        {
            // Dash controls movement completely - ignore input
        }

        public void OnEnd()
        {
            body.IsInputMovementEnabled = true;
        }
    }
}
