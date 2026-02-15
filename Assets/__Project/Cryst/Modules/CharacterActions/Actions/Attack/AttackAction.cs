using Gast.Domain.Characters;
using Gast.Features.Characters;
using Gast.Features.Combat;
using Cryst.Domain.Characters;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Cryst.Domain.Combat;

namespace Cryst.Modules.CharacterActions
{
    public class AttackAction : ICharacterExecutableAction<AttackCommand>
    {
        readonly CharacterContext context;
        readonly AttackActionSettings settings;
        readonly CharacterAnimator animator;
        readonly MeleeAttackEffect effect;
        readonly CharacterMovement movement;

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

            if (settings.EffectSettings != null)
            {
                effect = settings.EffectSettings.CreateEffect(context);
                effect.BindEvents().AddTo(context.Body.destroyCancellationToken);
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

            var damageArea = UnityEngine.Object.Instantiate(
                settings.DamageAreaPrefab,
                spawnPosition,
                spawnRotation);

            damageArea.transform.localScale = settings.HitboxSize;

            damageArea.Initialize(
                context.Id,
                settings.DamageAreaDuration);

            damageArea.Hit
                .Subscribe(OnAttackHit)
                .AddTo(damageArea.destroyCancellationToken);
        }

        void OnAttackHit(DamageHitInfo hit)
        {
            effect?.OnHit(hit);

            var hitActor = hit.Character.As<ICrystCharacter>();
            if (hitActor.Faction == context.Faction)
                return;

            var point = hit.Point;

            var knockbackDirection = point.rotation * Vector3.forward;
            var damageInfo = new DamageInfo(
                settings.Damage,
                point,
                settings.KnockbackForce * knockbackDirection,
                hit.AttackerId
            );

            hitActor.Hit(damageInfo);

            if (hitActor.IsAlive.Value)
            {
                hitActor.SetHealth(hitActor.Health.Value - damageInfo.Amount);

                if (hitActor.Health.Value <= 0)
                {
                    hitActor.Die();
                    hit.Character.DetachBrain();
                    context.EventPublisher.Publish(new CharacterDefeatedEvent(hitActor, damageInfo.AttackerId));
                    context.EventPublisher.Publish(
                        new LootSpawnEvent(
                            hit.Character.TypeDefinition.LootTable,
                            hit.Character.Body.Position));
                    hit.Character.DespawnAfterDelay().Forget();
                }
            }
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