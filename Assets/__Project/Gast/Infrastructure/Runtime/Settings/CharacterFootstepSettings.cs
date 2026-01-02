using UnityEngine;

namespace Gast.Infrastructure.Settings
{
    /// <summary>
    /// Configuration for character audio effects such as footsteps.
    /// Allows reusing audio settings across different character types.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterFootstepSettings", menuName = "DescrioGames/Characters/Footstep Settings")]
    public class CharacterFootstepSettings : ScriptableObject
    {
        [Header("Footsteps")]
        [SerializeField]
        AudioClip[] footstepClips = { };

        [SerializeField, Range(0f, 1f)]
        float footstepVolume = 0.5f;

        [Header("Randomness")]
        [Tooltip("Random volume variation range (±)")]
        [SerializeField, Range(0f, 0.5f)]
        float volumeVariance = 0.1f;

        [Tooltip("Random pitch variation range (±)")]
        [SerializeField, Range(0f, 0.5f)]
        float pitchVariance = 0.15f;

        public AudioClip[] FootstepClips => footstepClips;
        public float FootstepVolume => footstepVolume;
        public float VolumeVariance => volumeVariance;
        public float PitchVariance => pitchVariance;
    }
}