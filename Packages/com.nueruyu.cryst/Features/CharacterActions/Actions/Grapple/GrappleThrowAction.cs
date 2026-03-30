using System;
using Cryst.Domain.Characters.Commands;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    /// <summary>
    /// Attacker-side grapple throw action. Triggered by GrappleThrowCommand from
    /// GrappleHitHandler when the grapple area detects a victim. Disables attacker
    /// movement for the throw duration.
    /// </summary>
    public class GrappleThrowAction : ICharacterExecutableAction<GrappleThrowCommand>
    {
        readonly GrappleThrowActionSettings settings;
        readonly CharacterBody body;
        readonly CharacterAnimator animator;

        float startTime;

        public Type CommandType => typeof(GrappleThrowCommand);
        public int Priority => 7;

        public GrappleThrowAction(
            GrappleThrowActionSettings settings,
            CharacterBody body,
            CharacterAnimator animator)
        {
            this.settings = settings;
            this.body = body;
            this.animator = animator;
        }

        public bool CanExecute() => true;

        public void Execute(in GrappleThrowCommand command)
        {
            startTime = Time.time;
            body.IsInputMovementEnabled = false;

            // Placeholder: reuse attack animation until a dedicated throw animation is
            // added to CharacterAnimator (e.g. animator.PlayGrappleThrow())
            if (animator)
                animator.PlayAttack();
        }

        public bool OnUpdate()
        {
            return Time.time < startTime + settings.Duration;
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
