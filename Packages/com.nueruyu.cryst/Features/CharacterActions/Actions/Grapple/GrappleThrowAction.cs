using System;
using Cryst.Domain.Characters.Commands;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.Characters.IK;
using Gast.Unity.Shared.Attachments;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    /// <summary>
    /// Attacker-side grapple throw action. Triggered by GrappleThrowCommand from
    /// GrappleHitHandler when the grapple area detects a victim.
    /// Drives both the attacker animation and the victim's IK (victim's hand follows
    /// the attacker's configured hand anchor each frame).
    /// </summary>
    public class GrappleThrowAction : ICharacterExecutableAction<GrappleThrowCommand>
    {
        readonly GrappleThrowActionSettings settings;
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        readonly AttachmentAnchorRegistry anchorRegistry;

        float startTime;
        ICharacter victim;

        public Type CommandType => typeof(GrappleThrowCommand);
        public int Priority => 7;

        public GrappleThrowAction(
            GrappleThrowActionSettings settings,
            CharacterBody body,
            CharacterAnimator animator,
            AttachmentAnchorRegistry anchorRegistry)
        {
            this.settings = settings;
            this.body = body;
            this.animator = animator;
            this.anchorRegistry = anchorRegistry;
        }

        public bool CanExecute() => true;

        public void Execute(in GrappleThrowCommand command)
        {
            startTime = Time.time;
            victim = command.Victim;
            body.IsInputMovementEnabled = false;

            if (animator)
                animator.PlayTrigger(settings.ThrowTrigger);
        }

        public bool OnUpdate()
        {
            if (Time.time >= startTime + settings.Duration) return false;

            // Drive victim's hand IK toward the attacker's configured anchor each frame.
            if (victim != null
                && victim.Is(out CharacterIKController victimIK)
                && anchorRegistry.TryGetAnchor(settings.AttackerHandAnchor, out var handAnchor))
            {
                victimIK.SetIKTarget(settings.VictimIKGoal, handAnchor, 1f);
            }

            return true;
        }

        public void Move(Vector3 direction)
        {
        }

        public void OnEnd()
        {
            body.IsInputMovementEnabled = true;

            // Release victim's IK before clearing the reference.
            if (victim != null && victim.Is(out CharacterIKController victimIK))
                victimIK.SetIKTarget(settings.VictimIKGoal, null, 0f);

            victim = null;
        }
    }
}
