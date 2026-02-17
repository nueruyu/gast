using System;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Domain.Characters;
using Gast.Domain.Combat;
using UnityEngine;

namespace Gast.Features.Combat
{
    [RequireComponent(typeof(Collider))]
    public class HitArea : MonoBehaviour
    {
        IEffect effect;
        float duration;
        IDomainEventPublisher eventPublisher;

        bool initialized;

        public void Initialize(
            IEffect effect,
            float duration,
            IDomainEventPublisher eventPublisher)
        {
            this.effect = effect;
            this.duration = duration;
            this.eventPublisher = eventPublisher;
            initialized = true;

            DestroyAfterDelay().Forget();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!initialized)
                return;

            if (!other.TryGetComponent<ICharacter>(out var character))
                return;

            if (!effect.CanApplyTo(character))
                return;

            var hitPosition = other.ClosestPoint(transform.position);
            var hitRotation = Quaternion.LookRotation(transform.forward);
            var hitPoint = new Pose(hitPosition, hitRotation);

            eventPublisher.Publish(new CharacterHitEvent(character, hitPoint, effect));

            Debug.Log($"[HitArea] Hit: {character.Id}");
        }

        async UniTaskVoid DestroyAfterDelay()
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(duration),
                cancellationToken: destroyCancellationToken);

            Destroy(gameObject);

            var effect = this.effect;
            this.effect = null;

            if (effect is IDisposable disposableEffect)
            {
                disposableEffect.Dispose();
            }
        }
    }
}