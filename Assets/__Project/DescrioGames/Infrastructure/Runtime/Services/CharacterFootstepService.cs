using UnityEngine;
using DescrioGames.Core.Observables;
using Cysharp.Threading.Tasks;
using DescrioGames.Shared.Observables;
using R3;
using System;
using DisposableBag = DescrioGames.Core.Observables.DisposableBag;
using Random = UnityEngine.Random;
using DescrioGames.Infrastructure.Settings;

namespace DescrioGames.Features.Characters.Audios
{
    public class CharacterFootstepService : IDisposable
    {
        readonly DisposableBag disposableBag = new();

        public void Register(CharacterContext character, CharacterFootstepSettings settings)
        {
            character.AnimationReceiver.EventReceived
                .ToObservable()
                .Where(name => name == "Footstep")
                .Subscribe(_ => PlayFootstep(character, settings))
                .AddTo(disposableBag);
        }

        public void Dispose()
        {
            disposableBag.Dispose();
        }

        /// <summary>
        /// Plays a random footstep sound with volume and pitch variation.
        /// Retrieves sound settings from CharacterAudioSettings.
        /// </summary>
        void PlayFootstep(CharacterContext character, CharacterFootstepSettings settings)
        {
            var audioSource = character.Audio.AudioSource;

            // Select random footstep clip
            var clip = settings.FootstepClips[Random.Range(0, settings.FootstepClips.Length)];

            // Apply volume and pitch variation for natural sound
            audioSource.volume = settings.FootstepVolume + Random.Range(-settings.VolumeVariance, settings.VolumeVariance);
            audioSource.pitch = 1.0f + Random.Range(-settings.PitchVariance, settings.PitchVariance);

            audioSource.PlayOneShot(clip);
        }
    }
}