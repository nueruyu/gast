using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Domain.Combat;
using Gast.Domain.Stats;
using Gast.Features.Characters;
using Gast.Features.Combat;
using Gast.Shared.Observables;
using R3;
using System;
using System.Threading;
using UnityEngine;
using GastGame.Domain.Characters;

namespace GastGame.Features.CharacterActions
{
    public class MeleeMethod
    {
        readonly MeleeMethodSettings settings;
        readonly CharacterContext context;

        public MeleeMethod(
            MeleeMethodSettings settings,
            CharacterContext context)
        {
            this.settings = settings != null ? settings : throw new ArgumentNullException(nameof(settings));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IDisposable BindEvents(CharacterContext character)
        {
            var disposables = new CompositeDisposable();

            character.AnimationReceiver.EventReceived
                .ToObservable()
                .Where(name => name == "WeaponSwing")
                .Subscribe(_ =>
                {
                    PlayWeaponSwing(character.Audio.AudioSource);
                })
                .AddTo(disposables);

            return disposables;
        }

        public void Attack(CharacterContext attacker)
        {
            ExecuteAttackAsync(attacker, attacker.Body.destroyCancellationToken).Forget();
        }

        async UniTaskVoid ExecuteAttackAsync(CharacterContext attacker, CancellationToken cancellationToken)
        {
            var animator = attacker.Animator;
            if (animator)
                animator.PlayAttack();

            await UniTask.Delay(
                TimeSpan.FromSeconds(settings.AnimationTriggerDelay),
                cancellationToken: cancellationToken);

            var attackerTransform = attacker.Body.transform;

            var forward = attackerTransform.forward;
            var spawnPosition = attackerTransform.position + settings.Offset + forward * settings.Range;
            var spawnRotation = attackerTransform.rotation;

            var damageArea = UnityEngine.Object.Instantiate(
                settings.DamageAreaPrefab,
                spawnPosition,
                spawnRotation);

            damageArea.transform.localScale = settings.HitboxSize;

            damageArea.Initialize(
                attacker.Id,
                attacker.Faction,
                settings.Duration);

            damageArea.Hit
                .Subscribe(OnAttackHit)
                .AddTo(damageArea.destroyCancellationToken);
        }

        void OnAttackHit(DamageHitInfo hit)
        {
            var point = hit.Point;

            context.FeedbackService.PlayHitEffect(
                point.position,
                point.rotation,
                settings.HitVfxPrefab);

            context.FeedbackService.PlaySound(
                point.position,
                settings.HitSfx,
                settings.SfxVolume);

            var knockbackDirection = point.rotation * Vector3.forward;
            var damageInfo = new DamageInfo(
                settings.Damage,
                point,
                settings.KnockbackForce * knockbackDirection,
                hit.AttackerId
            );

            var hitActor = hit.Character.As<IGameCharacter>();

            hitActor.Hit(damageInfo);

            if (hitActor.IsAlive.Value)
            {
                hitActor.SetHealth(hitActor.Health.Value - damageInfo.Amount);

                if (hitActor.Health.Value <= 0)
                {
                    hitActor.Die();

                    hit.Character.DetachBrain();
                    context.EventPublisher.Publish(new CharacterDefeatedEvent(hit.Character, damageInfo.AttackerId));
                    hit.Character.DespawnAfterDelay().Forget();
                }
            }
        }

        void PlayWeaponSwing(AudioSource audioSource)
        {
            audioSource.volume = settings.SwingSfxVolume;
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(settings.SwingSfx);
        }
    }
}