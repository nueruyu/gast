using Gast.Features.Characters;
using System;
using UnityEngine;

namespace Cryst.Modules.CharacterActions
{
    public class HitAction : ICharacterExecutableAction<HitCommand>
    {
        readonly CharacterContext context;
        readonly HitActionSettings settings;
        readonly CharacterBody body;
        readonly CharacterAnimator animator;

        float startTime;
        Vector3 knockbackVelocity;

        public Type CommandType => typeof(HitCommand);
        public int Priority => 8;

        public HitAction(
            CharacterContext context,
            HitActionSettings settings,
            CharacterBody body,
            CharacterAnimator animator)
        {
            this.context = context;
            this.settings = settings;
            this.body = body;
            this.animator = animator;
        }

        public bool CanExecute() => true;

        public void Execute(in HitCommand command)
        {
            startTime = Time.time;
            knockbackVelocity = command.DamageInfo.KnockbackForce;

            if (animator)
                animator.PlayHit();

            body.SetForcedVelocity(knockbackVelocity);
            body.IsInputMovementEnabled = false;
        }

        public bool OnUpdate()
        {
            if (Time.time >= startTime + settings.Duration)
            {
                return false;
            }

            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, Time.deltaTime * settings.KnockbackFriction);
            body.SetForcedVelocity(knockbackVelocity);
            return true;
        }

        public void Move(Vector3 direction)
        {
        }

        public void OnEnd()
        {
            body.IsInputMovementEnabled = true;
            body.SetForcedVelocity(Vector3.zero);
        }
    }
}