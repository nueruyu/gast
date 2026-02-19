using Cryst.Domain.Combat;
using Cysharp.Threading.Tasks;
using Gast.Features.Characters;
using Gast.Features.Combat;
using System;
using System.Threading;
using UnityEngine;

namespace Cryst.Modules.CharacterActions
{
    public class AttackAction : ICharacterExecutableAction<AttackCommand>
    {
        readonly CharacterContext context;
        readonly AttackActionSettings settings;
        readonly CharacterAnimator animator;
        readonly CharacterMovement movement;
        readonly IHitAreaFactory hitAreaFactory;
        readonly IAttackEffectFactory attackEffectFactory;

        float startTime;
        float lastAttackTime = float.NegativeInfinity;

        public Type CommandType => typeof(AttackCommand);
        public int Priority => 5;

        public AttackAction(
            CharacterContext context,
            AttackActionSettings settings,
            CharacterAnimator animator,
            CharacterMovement movement,
            IHitAreaFactory hitAreaFactory,
            IAttackEffectFactory attackEffectFactory)
        {
            this.context = context;
            this.settings = settings;
            this.animator = animator;
            this.movement = movement;
            this.hitAreaFactory = hitAreaFactory;
            this.attackEffectFactory = attackEffectFactory;

            context.AnimationReceiver.EventReceived.Subscribe(OnAnimationEvent);
        }

        void OnAnimationEvent(string name)
        {
            if (name == "WeaponSwing")
            {
                var audioSource = context.Audio.AudioSource;
                audioSource.volume = settings.SfxVolume;
                audioSource.pitch = 1.0f;
                audioSource.PlayOneShot(settings.Sfx);
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

            ExecuteAttackAsync(context.Body.destroyCancellationToken).Forget();
        }

        async UniTaskVoid ExecuteAttackAsync(CancellationToken cancellationToken)
        {
            animator.PlayAttack();

            await UniTask.Delay(
                TimeSpan.FromSeconds(settings.AnimationTriggerDelay),
                cancellationToken: cancellationToken);

            var attackerTransform = context.Body.transform;
            var forward = attackerTransform.forward;
            var spawnPosition = attackerTransform.position + settings.Offset + forward * settings.Range;
            var spawnRotation = attackerTransform.rotation;
            var pose = new Pose(spawnPosition, spawnRotation);

            var knockbackDirection = spawnRotation * Vector3.forward;

            var effect = attackEffectFactory.Create(
                context.Id,
                settings.Damage,
                settings.KnockbackForce * knockbackDirection);

            hitAreaFactory.Create(
                pose,
                settings.HitboxSize,
                settings.DamageAreaDuration,
                effect);
        }

        public bool OnUpdate()
        {
            return Time.time < startTime + settings.Duration;
        }

        public void Move(Vector3 direction)
        {
            var speed = context.TypeDefinition.WalkSpeed;
            movement.Move(direction, speed);
        }

        public void OnEnd()
        {
        }
    }
}