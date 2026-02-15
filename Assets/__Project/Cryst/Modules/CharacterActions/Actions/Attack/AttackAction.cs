using Gast.Features.Characters;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Gast.Domain.Combat;
using Gast.Infrastructure.Combat;

namespace Cryst.Modules.CharacterActions
{
    public class AttackAction : ICharacterExecutableAction<AttackCommand>
    {
        readonly CharacterContext context;
        readonly AttackActionSettings settings;
        readonly CharacterAnimator animator;
        readonly CharacterMovement movement;
        readonly DamageAreaFactory damageAreaFactory;

        float startTime;
        float lastAttackTime = float.NegativeInfinity;

        public Type CommandType => typeof(AttackCommand);
        public int Priority => 5;

        public AttackAction(
            CharacterContext context,
            AttackActionSettings settings)
        {
            this.context = context;
            this.settings = settings;
            animator = context.Resolve<CharacterAnimator>();
            movement = context.Resolve<CharacterMovement>();
            damageAreaFactory = context.Resolve<DamageAreaFactory>();
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
            var attackInfo = new AttackInfo(
                context.Id,
                settings.Damage,
                settings.KnockbackForce * knockbackDirection
            );

            damageAreaFactory.Create(
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
            var speed = context.TypeDefinition.WalkSpeed;
            movement.Move(direction, speed);
        }

        public void OnEnd()
        {
        }
    }
}
