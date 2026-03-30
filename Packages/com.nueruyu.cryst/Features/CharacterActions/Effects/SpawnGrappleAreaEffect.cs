using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Effects
{
    /// <summary>
    /// An effect that spawns a short-lived grapple detection area.
    /// On hit, GrappleHitHandler coordinates the throw/grabbed state transition.
    /// </summary>
    [CreateAssetMenu(fileName = "SpawnGrappleAreaEffect", menuName = "Cryst/Action Effects/Spawn Grapple Area")]
    public class SpawnGrappleAreaEffect : CharacterActionEffect
    {
        [Header("Grapple Area")]
        [SerializeField]
        float duration = 0.2f;

        [SerializeField]
        float range = 1.0f;

        [SerializeField]
        Vector3 offset = Vector3.forward;

        [SerializeField]
        Vector3 hitboxSize = Vector3.one;

        public float Duration => duration;
        public float Range => range;
        public Vector3 Offset => offset;
        public Vector3 HitboxSize => hitboxSize;
    }
}
