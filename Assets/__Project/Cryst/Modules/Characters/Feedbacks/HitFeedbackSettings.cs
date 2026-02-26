using UnityEngine;

namespace Cryst.Modules.Characters.Feedbacks
{
    [CreateAssetMenu(menuName = "Cryst/Feedbacks/Hit Feedback Settings")]
    public class HitFeedbackSettings : ScriptableObject
    {
        [SerializeField]
        GameObject hitVfxPrefab;

        [SerializeField]
        AudioClip hitSfx;

        [SerializeField]
        float sfxVolume = 1.0f;

        public GameObject HitVfxPrefab => hitVfxPrefab;
        public AudioClip HitSfx => hitSfx;
        public float SfxVolume => sfxVolume;
    }
}