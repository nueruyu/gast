using System;
using Cysharp.Threading.Tasks;
using Gast.Domain.Characters;
using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Gast.Unity.Features.HitDetection
{
    [RequireComponent(typeof(Collider))]
    public class HitArea : MonoBehaviour
    {
        float duration;
        Action<ICharacter, Pose> onHit;

        bool initialized;

        public void Initialize(
            float duration,
            Action<ICharacter, Pose> onHit)
        {
            this.duration = duration;
            this.onHit = onHit;
            initialized = true;

            DestroyAfterDelay().Forget();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!initialized)
                return;

            if (!other.TryGetComponent<CharacterHost>(out var characterHost))
                return;

            var character = characterHost.Character;

            var hitPosition = other.ClosestPoint(transform.position);
            var hitRotation = Quaternion.LookRotation(transform.forward);
            var hitPoint = new Pose(hitPosition, hitRotation);

            onHit?.Invoke(character, hitPoint);

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
            onHit = null;
        }
    }
}
