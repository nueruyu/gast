using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Attachments;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Effects
{
    [CreateAssetMenu(fileName = "SpawnVfxEffect", menuName = "Cryst/Action Effects/Spawn VFX")]
    public class SpawnVfxEffect : CharacterActionEffect
    {
        [SerializeField]
        GameObject vfxPrefab;

        [SerializeField]
        AttachmentAnchorSymbol attachmentAnchorSymbol;

        [SerializeField]
        bool follow;

        [SerializeField]
        Vector3 offsetPosition = Vector3.zero;

        [SerializeField]
        Vector3 offsetRotation = Vector3.zero;

        [SerializeField]
        float duration = 2.0f;

        public GameObject VfxPrefab => vfxPrefab;
        public AttachmentAnchorSymbol AttachmentAnchorSymbol => attachmentAnchorSymbol;
        public bool Follow => follow;
        public Vector3 OffsetPosition => offsetPosition;
        public Vector3 OffsetRotation => offsetRotation;
        public float Duration => duration;
    }
}
