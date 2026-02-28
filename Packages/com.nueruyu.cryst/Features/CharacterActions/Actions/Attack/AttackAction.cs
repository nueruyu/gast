using System;
using System.Threading;
using Cryst.Domain.Characters.Commands;
using Cryst.Domain.Combat;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using Gast.Unity.Features.HitDetection;
using Gast.Unity.Infrastructure.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Attack
{
    public class AttackAction : ICharacterExecutableAction<AttackCommand>
    {
        readonly CharacterContext context;
        readonly AttackActionSettings settings;
        readonly CharacterBody body;
        readonly CharacterAnimator animator;
        readonly CharacterAudio audio;
        readonly CharacterMovement movement;
        readonly ICharacterTypeDefinition typeDefinition;
        readonly IHitAreaFactory hitAreaFactory;

        float startTime;
        float lastAttackTime = float.NegativeInfinity;

        public Type CommandType => typeof(AttackCommand);
        public int Priority => 5;

        public AttackAction(
            CharacterContext context,
            AttackActionSettings settings,
            CharacterBody body,
            CharacterAnimator animator,
            CharacterAudio audio,
            CharacterMovement movement,
            ICharacterTypeDefinition typeDefinition,
            IHitAreaFactory hitAreaFactory)
        {
            this.context = context;
            this.settings = settings;
            this.body = body;
            this.animator = animator;
            this.audio = audio;
            this.movement = movement;
            this.typeDefinition = typeDefinition;
            this.hitAreaFactory = hitAreaFactory;

            if (animator)
            {
                animator.AnimationEventReceiver.EventReceived.Subscribe(OnAnimationEvent);
            }
        }

        void OnAnimationEvent(string name)
        {
            if (name == "WeaponSwing")
            {
                var audioSource = audio.AudioSource;
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

            ExecuteAttackAsync(context.CancellationToken).Forget();
        }

        async UniTaskVoid ExecuteAttackAsync(CancellationToken cancellationToken)
        {
            animator.PlayAttack();

            await UniTask.Delay(
                TimeSpan.FromSeconds(settings.AnimationTriggerDelay),
                cancellationToken: cancellationToken);

            var attackerTransform = body.transform;
            var forward = attackerTransform.forward;
            var spawnPosition = attackerTransform.position + settings.Offset + forward * settings.Range;
            var spawnRotation = attackerTransform.rotation;
            var pose = new Pose(spawnPosition, spawnRotation);
            var knockbackDirection = spawnRotation * Vector3.forward;

            var attackInfo = new AttackInfo(
                context.CharacaterId,
                settings.Damage,
                settings.KnockbackForce * knockbackDirection);

            hitAreaFactory.Create(
                pose,
                settings.HitboxSize,
                settings.DamageAreaDuration,
                attackInfo);
        }

        public bool OnUpdate()
        {
            return Time.time < startTime + settings.Duration;
        }

        public void Move(Vector3 direction)
        {
            var movementSettings = typeDefinition.GetSettings<CharacterMovementSettings>();
            var speed = movementSettings.WalkSpeed;
            movement.Move(direction, speed);
        }

        public void OnEnd()
        {
        }
    }
}