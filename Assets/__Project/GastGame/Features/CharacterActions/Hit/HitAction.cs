using Gast.Features.Characters;
using GastGame.Features.Characters;
using System;
using UnityEngine;

namespace GastGame.Features.CharacterActions
{
    public class HitAction : ICharacterExecutableAction<HitCommand>
    {
        readonly CharacterContext context;
        readonly HitActionSettings settings;
        readonly CharacterAnimator animator;

        float startTime;
        Vector3 knockbackVelocity;

        public Type CommandType => typeof(HitCommand);
        public int Priority => 8;

        public HitAction(CharacterContext context, HitActionSettings settings)
        {
            this.context = context;
            this.settings = settings;
            animator = context.Resolve<CharacterAnimator>();
        }

        public bool CanExecute() => true;

        public void Execute(in HitCommand command)
        {
            startTime = Time.time;
            knockbackVelocity = command.DamageInfo.KnockbackForce;

            var body = context.Body;

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
            context.Body.SetForcedVelocity(knockbackVelocity);
            return true;
        }

        public void Move(Vector3 direction)
        {
        }

        public void OnEnd()
        {
            var body = context.Body;
            body.IsInputMovementEnabled = true;
            body.SetForcedVelocity(Vector3.zero);
        }
    }
}
