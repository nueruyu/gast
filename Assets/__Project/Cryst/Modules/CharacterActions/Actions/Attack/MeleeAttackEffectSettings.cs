using Gast.Features.Characters;
using UnityEngine;

namespace Cryst.Modules.CharacterActions
{
    [CreateAssetMenu(menuName = "Gast/Combat/Effect Settings/Melee")]
    public class MeleeAttackEffectSettings : ScriptableObject
    {
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

        public AudioClip SwingSfx => swingSfx;
        public float SwingSfxVolume => swingSfxVolume;
        public GameObject HitVfxPrefab => hitVfxPrefab;
        public AudioClip HitSfx => hitSfx;
        public float SfxVolume => sfxVolume;

        public MeleeAttackEffect CreateEffect(CharacterContext context)
        {
            return new MeleeAttackEffect(this, context);
        }
    }
}
