using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Attachments;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cryst.Features.CharacterActions.Effects
{
    public class SpawnVfxEffectHandler : ICharacterActionEffectHandler<SpawnVfxEffect>
    {
        readonly CharacterBody body;
        readonly AttachmentAnchorRegistry anchorRegistry;

        public SpawnVfxEffectHandler(CharacterBody body, AttachmentAnchorRegistry anchorRegistry)
        {
            this.body = body;
            this.anchorRegistry = anchorRegistry;
        }

        public void Handle(SpawnVfxEffect effect)
        {
            if (effect.VfxPrefab == null) return;

            Transform baseTransform;
            if (anchorRegistry.TryGetAnchor(effect.AttachmentAnchorSymbol, out var anchor))
            {
                baseTransform = anchor;
            }
            else
            {
                baseTransform = body.transform;
            }

            GameObject instance;
            if (effect.Follow)
            {
                instance = Object.Instantiate(effect.VfxPrefab, baseTransform);
                instance.transform.localPosition = effect.OffsetPosition;
                instance.transform.localRotation = Quaternion.Euler(effect.OffsetRotation);
            }
            else
            {
                var spawnPosition = baseTransform.position + baseTransform.rotation * effect.OffsetPosition;
                var spawnRotation = baseTransform.rotation * Quaternion.Euler(effect.OffsetRotation);
                instance = Object.Instantiate(effect.VfxPrefab, spawnPosition, spawnRotation);
            }

            if (effect.Duration > 0)
            {
                Object.Destroy(instance, effect.Duration);
            }
        }
    }
}
