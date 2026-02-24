using UnityEngine;

namespace Cryst.Infrastructure.Feedbacks
{
    public class CombatFeedbackService
    {
        public void PlayHitEffect(Vector3 position, Quaternion rotation, GameObject vfxPrefab)
        {
            if (vfxPrefab == null)
                return;

            var effectInstance = Object.Instantiate(vfxPrefab, position, rotation);

            Object.Destroy(effectInstance, 2.0f);
        }

        public void PlaySound(Vector3 position, AudioClip clip, float volume = 1.0f)
        {
            if (clip == null)
                return;

            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
    }
}
