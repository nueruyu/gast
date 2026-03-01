using UnityEngine;

namespace Cryst.Features.CharacterActions.Effects
{
    /// <summary>
    ///     An effect that spawns a hit detection area.
    /// </summary>
    [CreateAssetMenu(fileName = "SpawnHitAreaEffect", menuName = "Cryst/Action Effects/Spawn Hit Area")]
    public class SpawnHitAreaEffect : CharacterActionEffect
    {
        [Header("Damage Area")] [SerializeField]
        float duration = 0.3f;

        [SerializeField] float range = 1.5f;

        [SerializeField] Vector3 offset = Vector3.zero;

        [SerializeField] Vector3 hitboxSize = Vector3.one;

        [Header("Impact")] [SerializeField] float damage = 10f;

        [SerializeField] float knockbackForce = 5f;

        public float Duration => duration;
        public float Range => range;
        public Vector3 Offset => offset;
        public Vector3 HitboxSize => hitboxSize;
        public float Damage => damage;
        public float KnockbackForce => knockbackForce;
    }
}