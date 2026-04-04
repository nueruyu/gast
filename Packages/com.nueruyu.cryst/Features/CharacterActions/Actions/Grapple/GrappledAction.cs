using System;
using Cryst.Domain.Characters;
using Cryst.Domain.Characters.Commands;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.Characters.IK;
using Gast.Unity.Shared.Attachments;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    /// <summary>
    /// Victim-side grapple action. Triggered by GrappledCommand from GrappleHitHandler.
    /// Disables input movement, moves the victim toward the attacker each frame, and
    /// drives its own IK goals toward anchors on the attacker (configured via Settings).
    /// </summary>
    public class GrappledAction : ICharacterExecutableAction<GrappledCommand>
    {
        readonly GrappledActionSettings settings;
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        readonly CharacterIKController ikController;

        ICharacter attacker;
        float startTime;

        public Type CommandType => typeof(GrappledCommand);
        public int Priority => 9;

        public GrappledAction(
            GrappledActionSettings settings,
            CharacterBody body,
            CharacterAnimator animator,
            CharacterIKController ikController)
        {
            this.settings = settings;
            this.body = body;
            this.animator = animator;
            this.ikController = ikController;
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

            if (delta.sqrMagnitude > 0.0025f)
                body.SetForcedVelocity(delta / Time.deltaTime);

            if (ikController != null
                && attacker.Is(out AttachmentAnchorRegistry attackerAnchors))
            {
                foreach (var binding in settings.IKBindings)
                {
                    if (attackerAnchors.TryGetAnchor(binding.AttackerAnchor, out var anchor))
                        ikController.SetIKTarget(binding.SelfGoal, anchor, 1f);
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

            if (ikController != null)
            {
                foreach (var binding in settings.IKBindings)
                    ikController.SetIKTarget(binding.SelfGoal, null, 0f);
            }

            attacker = null;
        }
    }
}
