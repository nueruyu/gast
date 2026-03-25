using UnityEngine;

namespace Cryst.Features.Characters.Feedbacks
{
    public class CharacterFeedbackService
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

            var gameObject = new GameObject("One shot audio");
            gameObject.transform.position = position;
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.minDistance = 10;
            audioSource.maxDistance = 50;
            audioSource.volume = volume;
            audioSource.Play();
            Object.Destroy(gameObject, clip.length);
        }
    }
}
