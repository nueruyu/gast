using Cysharp.Threading.Tasks;
using DescrioGames.Features.Characters;
using DescrioGames.Features.Combat.Methods;
using R3;
using UnityEngine;

namespace DescrioGames.Features.Combat.MethodSettings
{
    /// <summary>
    /// Configuration for melee attack behavior.
    /// </summary>
    [CreateAssetMenu(menuName = "DescrioGames/Combat/Method Settings/Melee")]
    public class MeleeMethodSettings : CombatMethodSettings
    {
        [Header("Damage Area")]
        [SerializeField]
        DamageArea damageAreaPrefab;

        [SerializeField]
        float duration = 0.3f;

        [SerializeField]
        float range = 1.5f;

        [SerializeField]
        Vector3 offset = Vector3.zero;

        [SerializeField]
        Vector3 hitboxSize = new(1f, 1f, 1f);

        [SerializeField]
        float animationTriggerDelay = 0.2f;

        [Header("Impact")]
        [SerializeField]
        float damage = 10f;

        [SerializeField]
        float knockbackForce = 5f;

        [Header("Feedback")]
        [SerializeField]
        AudioClip swingSfx;

        [SerializeField]
        float swingSfxVolume = 1.0f;

        [SerializeField]
        GameObject hitVfxPrefab;

        [SerializeField]
        AudioClip hitSfx;

        [SerializeField]
        float sfxVolume = 1.0f;

        public DamageArea DamageAreaPrefab => damageAreaPrefab;
        public float Duration => duration;
        public float Range => range;
        public Vector3 Offset => offset;
        public Vector3 HitboxSize => hitboxSize;
        public float AnimationTriggerDelay => animationTriggerDelay;

        public float Damage => damage;
        public float KnockbackForce => knockbackForce;

        public AudioClip SwingSfx => swingSfx;
        public float SwingSfxVolume => swingSfxVolume;

        public GameObject HitVfxPrefab => hitVfxPrefab;
        public AudioClip HitSfx => hitSfx;
        public float SfxVolume => sfxVolume;

        public override ICombatMethod CreateMethod(CombatContext context)
        {
            return new MeleeMethod(this, context);
        }
    }
}