using Gast.Features.Characters;
using System;
using UnityEngine;

namespace Cryst.Modules.CharacterActions
{
    public class DashAction : ICharacterExecutableAction<DashCommand>
    {
        readonly CharacterContext context;
        readonly DashActionSettings settings;
        readonly CharacterBody body;
        readonly CharacterAnimator animator;

        float startTime;
        float lastDashTime = float.NegativeInfinity;
        Vector3 dashDirection;

        public Type CommandType => typeof(DashCommand);
        public int Priority => 10;

        public DashAction(
            CharacterContext context,
            DashActionSettings settings,
            CharacterBody body,
            CharacterAnimator animator)
        {
            this.context = context;
            this.settings = settings;
            this.body = body;
            this.animator = animator;
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
            body.SetLookDirection(dashDirection, settings.LookDirectionSpeed);

            return true;
        }

        public void Move(Vector3 direction)
        {
        }

        public void OnEnd()
        {
            body.IsInputMovementEnabled = true;
        }
    }
}