using System;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Domain.Characters;
using UnityEngine;

namespace Gast.Features.Combat
{
    [RequireComponent(typeof(Collider))]
    public class HitArea : MonoBehaviour
    {
        object context;
        float duration;
        IDomainEventPublisher eventPublisher;

        bool initialized;

        public void Initialize(
            object context,
            float duration,
            IDomainEventPublisher eventPublisher)
        {
            this.context = context;
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

            var hitPosition = other.ClosestPoint(transform.position);
            var hitRotation = Quaternion.LookRotation(transform.forward);
            var hitPoint = new Pose(hitPosition, hitRotation);

            eventPublisher.Publish(new CharacterHitEvent(character, hitPoint, context));

            Debug.Log($"[HitArea] Hit: {character.Id}");
        }

        async UniTaskVoid DestroyAfterDelay()
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(duration),
                cancellationToken: destroyCancellationToken);

            Destroy(gameObject);
        }

        void OnDestroy()
        {
            if (context is IDisposable disposableContext)
            {
                disposableContext.Dispose();
            }
            context = null;
        }
    }
}