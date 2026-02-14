using Gast.Domain.Characters;
using Gast.Features.Characters;
using Gast.Features.Combat;
using GastGame.Domain.Characters;
using GastGame.Features.Characters;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using GastGame.Domain.Combat;

namespace GastGame.Features.CharacterActions
{
    public class AttackAction : ICharacterExecutableAction<AttackCommand>
    {
        readonly CharacterContext context;
        readonly AttackActionSettings settings;
        readonly CharacterAnimator animator;
        readonly MeleeAttackEffect effect;

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

            var hitActor = hit.Character.As<IGameCharacter>();
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
                    context.EventPublisher.Publish(new CharacterDefeatedEvent(hit.Character, damageInfo.AttackerId));
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
            if (direction.sqrMagnitude > 0.01f)
            {
                var speed = context.TypeDefinition.WalkSpeed;
                context.Body.SetInputVelocity(direction * speed);
                context.Body.SetLookDirection(direction, 10f);
            }
        }

        public void OnEnd()
        {
        }
    }
}