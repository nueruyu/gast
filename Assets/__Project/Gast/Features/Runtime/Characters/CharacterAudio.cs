using UnityEngine;

namespace DescrioGames.Features.Characters
{
    public class CharacterAudio : MonoBehaviour
    {
        [SerializeField]
        AudioSource audioSource;

        public AudioSource AudioSource => audioSource;
    }
}