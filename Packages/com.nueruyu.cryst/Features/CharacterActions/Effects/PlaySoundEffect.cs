using Gast.Unity.Features.Characters;
using UnityEngine;

namespace Cryst.Features.CharacterActions.Effects
{
    /// <summary>
    ///     An effect that plays a one-shot sound clip.
    /// </summary>
    [CreateAssetMenu(fileName = "PlaySoundEffect", menuName = "Cryst/Action Effects/Play Sound")]
    public class PlaySoundEffect : CharacterActionEffect
    {
        [SerializeField] AudioClip sfx;

        [SerializeField] [Range(0f, 1f)] float volume = 1.0f;

        public AudioClip Sfx => sfx;
        public float Volume => volume;
    }
}