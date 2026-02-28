using UnityEngine;

namespace Gast.Unity.Features.Characters
{
    public class CharacterAudio : MonoBehaviour
    {
        [SerializeField]
        AudioSource audioSource;

        public AudioSource AudioSource => audioSource;
    }
}