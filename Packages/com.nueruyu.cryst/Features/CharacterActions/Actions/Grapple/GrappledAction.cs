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
    /// If a CharacterIKController is present on the character, it activates IK weight
    /// for the left hand to simulate being grabbed.
    /// </summary>
    public class GrappledAction : ICharacterExecutableAction<GrappledCommand>
    {
        readonly GrappledActionSettings settings;
        readonly CharacterBody body;
        // Resolved at construction time from the character hierarchy; may be null if
        // IK rig has not been set up on this character.
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
            // Walk up the hierarchy from CharacterBody to find the IK controller on the
            // root GameObject (placed there by IKRigBuilder).
            ikController = body.transform.root.GetComponent<CharacterIKController>();
        }

        public bool CanExecute() => true;

        public void Execute(in GrappledCommand command)
        {
            attacker = command.Attacker;
            startTime = Time.time;
            body.IsInputMovementEnabled = false;

            // Activate IK weight for the grabbed hand at current world position.
            // Passing null as target keeps the IK target where it is; only the weight changes.
            ikController?.SetIKTarget(AvatarIKGoal.LeftHand, null, 1f);
        }

        public bool OnUpdate()
        {
            if (attacker == null) return false;
            if (Time.time >= startTime + settings.Duration) return false;

            // Move victim to stay in front of the attacker using forced velocity so it
            // ignores the IsInputMovementEnabled = false flag.
            var attackerBody = attacker.As<BaseCharacter>().Body;
            var targetPos = attackerBody.Position + attackerBody.Forward * 0.6f;
            var delta = targetPos - body.Position;

            if (delta.sqrMagnitude > 0.0025f) // 0.05m dead-zone
            {
                // Divide by deltaTime to express as velocity; CharacterBody multiplies by
                // deltaTime internally, effectively snapping to target this frame.
                var snap = new Vector3(delta.x, 0f, delta.z) / Time.deltaTime;
                body.SetForcedVelocity(snap);
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
