using System;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Commands;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.Characters.IK;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    /// <summary>
    /// Victim-side grapple action. Triggered by GrappledCommand from GrappleHitHandler.
    /// Disables input movement and moves the victim toward the attacker each frame.
    /// If a CharacterIKController is present, it activates IK weight for the left hand
    /// to simulate being grabbed.
    /// </summary>
    public class GrappledAction : ICharacterExecutableAction<GrappledCommand>
    {
        readonly GrappledActionSettings settings;
        readonly CharacterBody body;
        // Resolved from the character root at construction time; null when IK rig is absent.
        readonly CharacterIKController ikController;

        ICharacter attacker;
        float startTime;

        public Type CommandType => typeof(GrappledCommand);

        // High priority so it overrides normal combat actions while grabbed
        public int Priority => 9;

        public GrappledAction(GrappledActionSettings settings, CharacterBody body)
        {
            this.settings = settings;
            this.body = body;
            // IKRigBuilder places CharacterIKController on the root GameObject.
            ikController = body.transform.root.GetComponent<CharacterIKController>();
        }

        public bool CanExecute() => true;

        public void Execute(in GrappledCommand command)
        {
            attacker = command.Attacker;
            startTime = Time.time;
            body.IsInputMovementEnabled = false;

            // Passing null keeps the IK target at its current world position; only weight changes.
            ikController?.SetIKTarget(AvatarIKGoal.LeftHand, null, 1f);
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
                // delta / deltaTime expresses the displacement as velocity;
                // CharacterBody.ApplyPhysics multiplies by deltaTime, snapping to target each frame.
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
            ikController?.SetIKTarget(AvatarIKGoal.LeftHand, null, 0f);
            attacker = null;
        }
    }
}
