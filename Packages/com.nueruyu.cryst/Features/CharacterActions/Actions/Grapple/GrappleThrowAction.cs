using System;
using Cryst.Domain.Characters.Commands;
using Cryst.Features.Characters;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Attachments;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    /// <summary>
    /// Attacker-side grapple throw action. Triggered by GrappleThrowCommand from
    /// GrappleHitHandler when the grapple area detects a victim.
    /// Drives the attacker animation and drives the victim's IK goals toward
    /// the attacker's configured anchors each frame.
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

            if (victim != null
                && victim.Is(out RiggedCharacter victimRig)
                && victimRig.IKController != null)
            {
                foreach (var binding in settings.IKBindings)
                {
                    if (anchorRegistry.TryGetAnchor(binding.AttackerAnchor, out var anchor))
                        victimRig.IKController.SetIKTarget(binding.VictimGoal, anchor, 1f);
                }
            }

            return true;
        }

        public void Move(Vector3 direction)
        {
        }

        public void OnEnd()
        {
            body.IsInputMovementEnabled = true;

            if (victim != null
                && victim.Is(out RiggedCharacter victimRig)
                && victimRig.IKController != null)
            {
                foreach (var binding in settings.IKBindings)
                    victimRig.IKController.SetIKTarget(binding.VictimGoal, null, 0f);
            }

            victim = null;
        }
    }
}
