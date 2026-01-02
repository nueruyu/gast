using UnityEngine;

namespace Gast.Features.Characters
{
    public class CharacterAudio : MonoBehaviour
    {
        [SerializeField]
        AudioSource audioSource;

        public AudioSource AudioSource => audioSource;
    }
}