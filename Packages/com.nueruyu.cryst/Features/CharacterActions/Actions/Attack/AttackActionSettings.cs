using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Animations;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Actions.Attack
{
    [CreateAssetMenu(fileName = "AttackActionSettings", menuName = "Gast/Actions/Attack Action Settings")]
    public class AttackActionSettings : CharacterActionSettings
    {
        [Header("Core")]
        [SerializeField]
        float cooldown = 1f;

        [SerializeField]
        float duration = 0.6f;

        [Header("Damage Area")]
        [SerializeField]
        float damageAreaDuration = 0.3f;

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
        AudioClip sfx;

        [SerializeField]
        float sfxVolume = 1.0f;

        [SerializeField]
        AnimationEventSymbol sfxEvent;

        public float Cooldown => cooldown;
        public float Duration => duration;
        public float DamageAreaDuration => damageAreaDuration;
        public float Range => range;
        public Vector3 Offset => offset;
        public Vector3 HitboxSize => hitboxSize;
        public float AnimationTriggerDelay => animationTriggerDelay;
        public float Damage => damage;
        public float KnockbackForce => knockbackForce;
        public AudioClip Sfx => sfx;
        public float SfxVolume => sfxVolume;
        public AnimationEventSymbol SfxEvent => sfxEvent;
    }
}
