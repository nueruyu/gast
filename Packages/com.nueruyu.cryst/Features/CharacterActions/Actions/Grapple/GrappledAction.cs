using System;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Commands;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    /// <summary>
    /// Victim-side grapple action. Triggered by GrappledCommand from GrappleHitHandler.
    /// Disables input movement and moves the victim toward the attacker each frame.
    /// IK control is the attacker's responsibility (GrappleThrowAction).
    /// </summary>
    public class GrappledAction : ICharacterExecutableAction<GrappledCommand>
    {
        readonly GrappledActionSettings settings;
        readonly CharacterBody body;
        readonly CharacterAnimator animator;

        ICharacter attacker;
        float startTime;

        public Type CommandType => typeof(GrappledCommand);
        public int Priority => 9;

        public GrappledAction(GrappledActionSettings settings, CharacterBody body, CharacterAnimator animator)
        {
            this.settings = settings;
            this.body = body;
            this.animator = animator;
        }

        public bool CanExecute() => true;

        public void Execute(in GrappledCommand command)
        {
            attacker = command.Attacker;
            startTime = Time.time;
            body.IsInputMovementEnabled = false;

            if (animator)
                animator.PlayTrigger(settings.GrappledTrigger);
        }

        public bool OnUpdate()
        {
            if (attacker == null) return false;
            if (Time.time >= startTime + settings.Duration) return false;

            var attackerBody = attacker.As<BaseCharacter>().Body;
            var targetPos = attackerBody.Position + attackerBody.Forward * 0.6f;
            var delta = targetPos - body.Position;

            if (delta.sqrMagnitude > 0.0025f) // 0.05 m dead-zone
            {
                // delta / deltaTime expresses displacement as velocity;
                // CharacterBody.ApplyPhysics multiplies by deltaTime, snapping to target this frame.
                body.SetForcedVelocity(delta / Time.deltaTime);
            }

            return true;
        }

        public void Move(Vector3 direction)
        {
        }

        public void OnEnd()
        {
            body.IsInputMovementEnabled = true;
            attacker = null;
        }
    }
}
