using System;
using System.Linq;
using Cryst.Domain.Characters.Commands;
using Cryst.Features.CharacterActions.Effects;
using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Animations;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Grapple
{
    /// <summary>
    /// Grapple initiation action. Spawns a grapple detection area via
    /// SpawnGrappleAreaEffect when the animation event fires.
    /// On hit, GrappleHitHandler transitions attacker to GrappleThrowAction
    /// and victim to GrappledAction.
    /// </summary>
    public class GrappleAction : ICharacterExecutableAction<GrappleCommand>
    {
        readonly GrappleActionSettings settings;
        readonly CharacterAnimator animator;
        readonly CharacterMovement movement;
        readonly CharacterMovementSettings movementSettings;
        readonly CharacterActionEffectDispatcher effectDispatcher;

        float startTime;
        float lastAttackTime = float.NegativeInfinity;

        public Type CommandType => typeof(GrappleCommand);
        public int Priority => 5;

        public GrappleAction(
            GrappleActionSettings settings,
            CharacterAnimator animator,
            CharacterMovement movement,
            CharacterMovementSettings movementSettings,
            CharacterActionEffectDispatcher effectDispatcher)
        {
            this.settings = settings;
            this.animator = animator;
            this.movement = movement;
            this.movementSettings = movementSettings;
            this.effectDispatcher = effectDispatcher;

            if (animator)
            {
                animator.AnimationEventReceiver.EventReceived.Subscribe(OnAnimationEvent);
            }
        }

        void OnAnimationEvent(AnimationEventSymbol eventSymbol)
        {
            var timedEffect = settings.TimedEffects.FirstOrDefault(e => e.EventSymbol == eventSymbol);
            if (timedEffect?.Effect != null)
            {
                effectDispatcher.Dispatch(timedEffect.Effect);
            }
        }

        public bool CanExecute()
        {
            return Time.time >= lastAttackTime + settings.Cooldown;
        }

        public void Execute(in GrappleCommand command)
        {
            startTime = Time.time;
            lastAttackTime = startTime;

            animator.PlayAttack();
        }

        public bool OnUpdate()
        {
            return Time.time < startTime + settings.Duration;
        }

        public void Move(Vector3 direction)
        {
            movement.Move(direction, movementSettings.WalkSpeed);
        }

        public void OnEnd()
        {
        }
    }
}
