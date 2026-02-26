using UnityEngine;
using Gast.Core.Observables;
using Cysharp.Threading.Tasks;
using Gast.Shared.Observables;
using R3;
using System;
using DisposableBag = Gast.Core.Observables.DisposableBag;
using Random = UnityEngine.Random;
using Gast.Features.Characters;
using Cryst.Modules.CharacterActions;

namespace Cryst.Modules.Characters.Footsteps
{
    public class CharacterFootstepHandler : IDisposable
    {
        readonly DisposableBag disposableBag = new();

        public CharacterFootstepHandler(
            CharacterAnimator animator,
            CharacterAudio audio,
            CharacterFootstepSettings settings)
        {
            animator.AnimationEventReceiver.EventReceived
                .ToObservable()
                .Where(name => name == "Footstep")
                .Subscribe(_ => PlayFootstep(audio, settings))
                .AddTo(disposableBag);
        }

        public void Dispose()
        {
            disposableBag.Dispose();
        }

        void PlayFootstep(CharacterAudio audio, CharacterFootstepSettings settings)
        {
            var audioSource = audio.AudioSource;

            // Select random footstep clip
            var clip = settings.FootstepClips[Random.Range(0, settings.FootstepClips.Length)];

            // Apply volume and pitch variation for natural sound
            audioSource.volume = settings.FootstepVolume + Random.Range(-settings.VolumeVariance, settings.VolumeVariance);
            audioSource.pitch = 1.0f + Random.Range(-settings.PitchVariance, settings.PitchVariance);

            audioSource.PlayOneShot(clip);
        }
    }
}