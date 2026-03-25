using UnityEngine;

namespace Gast.Unity.Features.Characters
{
    public class CharacterAudio : MonoBehaviour
    {
        [SerializeField]
        AudioSource oneShotAudioSource;
        
        [SerializeField]
        AudioSource footStepAudioSource;

        public AudioSource OneShotAudioSource => oneShotAudioSource;

        public AudioSource FootStepAudioSource => footStepAudioSource;
    }
}