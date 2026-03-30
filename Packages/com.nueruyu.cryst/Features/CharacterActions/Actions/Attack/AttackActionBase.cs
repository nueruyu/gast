using System;
using System.Linq;
using Cryst.Features.CharacterActions.Effects;
using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Animations;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Attack
{
    /// <summary>
    /// Shared base for actions that use the AttackActionSettings pipeline:
    /// cooldown guard, duration tracking, and timed animation-event effects.
    /// CommandType is abstract so each subclass binds to its own command type
    /// without C# interface-dispatch ambiguity.
    /// </summary>
    public abstract class AttackActionBase : ICharacterExecutableAction
    {
        readonly AttackActionSettings settings;
        readonly CharacterAnimator animator;
        readonly CharacterMovement movement;
        readonly CharacterMovementSettings movementSettings;
        readonly CharacterActionEffectDispatcher effectDispatcher;

        float startTime;
        float lastAttackTime = float.NegativeInfinity;

        public abstract Type CommandType { get; }
        public int Priority => 5;

        protected AttackActionBase(
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
                animator.AnimationEventReceiver.EventReceived.Subscribe(OnAnimationEvent);
        }

        void OnAnimationEvent(AnimationEventSymbol eventSymbol)
        {
            var timedEffect = settings.TimedEffects.FirstOrDefault(e => e.EventSymbol == eventSymbol);
            if (timedEffect?.Effect != null)
                effectDispatcher.Dispatch(timedEffect.Effect);
        }

        public bool CanExecute() => Time.time >= lastAttackTime + settings.Cooldown;

        protected void BeginAttack()
        {
            startTime = Time.time;
            lastAttackTime = startTime;
            animator.PlayAttack();
        }

        public bool OnUpdate() => Time.time < startTime + settings.Duration;

        public void Move(Vector3 direction) => movement.Move(direction, movementSettings.WalkSpeed);

        public void OnEnd() { }
    }
}
