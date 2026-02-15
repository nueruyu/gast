using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Gast.Core.Observables;
using Gast.Domain.Characters;
using Gast.Features.Characters;
using UnityEngine;

namespace Gast.Features.Combat
{
    /// <summary>
    /// Temporary collision detection object for attacks.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DamageArea : MonoBehaviour
    {
        readonly Signal<DamageHitInfo> hit = new();

        CharacterId ownerId;
        float duration;

        bool initialized;

        public ISignal<DamageHitInfo> Hit => hit;

        public void Initialize(
            CharacterId ownerId,
            float duration)
        {
            this.ownerId = ownerId;
            this.duration = duration;
            initialized = true;

            DestroyAfterDelay().Forget();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!initialized)
                return;

            if (!other.TryGetComponent<Character>(out var character))
                return;

            var hitPosition = other.ClosestPoint(transform.position);
            var hitRotation = Quaternion.LookRotation(transform.forward);
            var hitPoint = new Pose(hitPosition, hitRotation);

            hit.Publish(new(ownerId, character, hitPoint));

            Debug.Log($"[DamageArea] Hit: {character.Id} (Owner: {ownerId})");
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