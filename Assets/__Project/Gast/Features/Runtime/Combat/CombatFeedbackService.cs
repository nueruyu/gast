using UnityEngine;

namespace DescrioGames.Features.Combat
{
    /// <summary>
    /// Service for managing combat feedback effects (VFX and SFX).
    /// </summary>
    public class CombatFeedbackService
    {
        /// <summary>
        /// Play a hit effect at the specified position and rotation.
        /// </summary>
        /// <param name="position">World position to spawn the effect</param>
        /// <param name="rotation">Rotation of the effect</param>
        /// <param name="vfxPrefab">VFX prefab to instantiate</param>
        public void PlayHitEffect(Vector3 position, Quaternion rotation, GameObject vfxPrefab)
        {
            if (vfxPrefab == null)
                return;

            var effectInstance = Object.Instantiate(vfxPrefab, position, rotation);

            Object.Destroy(effectInstance, 2.0f);
        }

        /// <summary>
        /// Play a sound at the specified position.
        /// </summary>
        /// <param name="position">World position to play the sound</param>
        /// <param name="clip">Audio clip to play</param>
        /// <param name="volume">Volume of the sound (0.0 to 1.0)</param>
        public void PlaySound(Vector3 position, AudioClip clip, float volume = 1.0f)
        {
            if (clip == null)
                return;

            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
    }
}