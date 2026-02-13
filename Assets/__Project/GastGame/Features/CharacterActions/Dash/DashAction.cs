using Gast.Features.Characters;
using GastGame.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    /// <summary>
    /// Dash action with curve-driven movement.
    /// Disables input movement and applies forced velocity based on animation curve.
    /// </summary>
    public class DashAction : ICharacterAction<DashCommand>
    {
        readonly CharacterActionContext context;
        readonly DashActionSettings settings;

        float startTime;
        float lastDashTime = float.NegativeInfinity;
        Vector3 dashDirection;

        public Type CommandType => typeof(DashCommand);
        public int Priority => 10;

        public DashAction(
            CharacterActionContext context,
            DashActionSettings settings)
        {
            this.context = context;
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

            var body = context.CharacterContext.Body;
            body.IsInputMovementEnabled = false;

            dashDirection = body.Forward;

            var animator = context.CharacterAnimator;
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

            var body = context.CharacterContext.Body;
            float speedEval = settings.SpeedCurve.Evaluate(progress);
            body.SetForcedVelocity(dashDirection * (settings.MaxSpeed * speedEval));
            body.SetLookDirection(dashDirection, settings.LookDirectionSpeed);

            return true;
        }

        public void Move(Vector3 direction)
        {
            // Dash controls movement completely - ignore input
        }

        public void OnEnd()
        {
            context.CharacterContext.Body.IsInputMovementEnabled = true;
        }
    }
}
