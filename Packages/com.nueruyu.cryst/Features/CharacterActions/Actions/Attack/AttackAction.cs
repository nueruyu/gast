using System;
using System.Linq;
using Cryst.Domain.Characters.Commands;
using Cryst.Features.CharacterActions.Effects;
using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Animations;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Attack
{
    public class AttackAction : ICharacterExecutableAction<AttackCommand>
    {
        readonly AttackActionSettings settings;
        readonly CharacterAnimator animator;
        readonly CharacterMovement movement;
        readonly CharacterMovementSettings movementSettings;
        readonly CharacterActionEffectDispatcher effectDispatcher;

        float startTime;
        float lastAttackTime = float.NegativeInfinity;

        public Type CommandType => typeof(AttackCommand);
        public int Priority => 5;

        public AttackAction(
            AttackActionSettings settings,
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

        public void Execute(in AttackCommand command)
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
            var speed = movementSettings.WalkSpeed;
            movement.Move(direction, speed);
        }

        public void OnEnd()
        {
        }
    }
}
