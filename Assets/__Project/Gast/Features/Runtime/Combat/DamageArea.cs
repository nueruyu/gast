using System;
using Cysharp.Threading.Tasks;
using Gast.Core.Events;
using Gast.Domain.Characters;
using Gast.Domain.Combat;
using Gast.Features.Characters;
using UnityEngine;

namespace Gast.Features.Combat
{
    [RequireComponent(typeof(Collider))]
    public class DamageArea : MonoBehaviour
    {
        AttackInfo attackInfo;
        float duration;
        IDomainEventPublisher eventPublisher;

        bool initialized;

        public void Initialize(
            AttackInfo attackInfo,
            float duration,
            IDomainEventPublisher eventPublisher)
        {
            this.attackInfo = attackInfo;
            this.duration = duration;
            this.eventPublisher = eventPublisher;
            initialized = true;

            DestroyAfterDelay().Forget();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!initialized)
                return;

            if (!other.TryGetComponent<Character>(out var character))
                return;

            if (character.Id == attackInfo.AttackerId)
                return;

            var hitPosition = other.ClosestPoint(transform.position);
            var hitRotation = Quaternion.LookRotation(transform.forward);
            var hitPoint = new Pose(hitPosition, hitRotation);

            eventPublisher.Publish(new CharacterDamagedEvent(character, attackInfo, hitPoint));

            Debug.Log($"[DamageArea] Hit: {character.Id} (Owner: {attackInfo.AttackerId})");
        }

        async UniTaskVoid DestroyAfterDelay()
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(duration),
                cancellationToken: destroyCancellationToken);

            Destroy(gameObject);
        }
    }
}
