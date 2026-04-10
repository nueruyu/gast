using System;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Commands;
using Cryst.Features.Characters;
using Cryst.Features.Characters.Facets;
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

            UpdateGrappledCharacter();
            if (animator)
                animator.PlayTrigger(settings.ThrowTrigger);
            
        }

        public bool OnUpdate()
        {
            if (Time.time >= startTime + settings.Duration)
                return false;

            if (victim != null &&
                victim.Is(out RiggedCharacter victimRig) &&
                victimRig.IKController != null)
            {
                foreach (var binding in settings.IKBindings)
                {
                    if (anchorRegistry.TryGetAnchor(binding.AttackerAnchor, out var anchor))
                        victimRig.IKController.SetIKTargetPose(binding.VictimGoal, anchor);
                }
            }

            return true;
        }

        void UpdateGrappledCharacter()
        {
            if (victim == null) 
                return;
            var victimBody = victim.As<BaseCharacter>().Body as CharacterBody;
            if (victimBody == null) 
                return;

            var targetPosition = body.transform.TransformPoint(settings.GrappledPositionOffset);
            var targetRotation = body.transform.rotation * Quaternion.Euler(settings.GrappledRotationOffset);
            victimBody.SetPosition(targetPosition);
            victimBody.SetRotation(targetRotation);
        }

        public void Move(Vector3 direction)
        {
        }

        public void OnEnd()
        {
            body.IsInputMovementEnabled = true;
            victim = null;
        }
    }
}
