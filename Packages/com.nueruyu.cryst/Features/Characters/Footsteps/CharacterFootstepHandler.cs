using System;
using Cryst.Features.CharacterActions;
using Gast.Core.Observables;
using Gast.Unity.Features.Characters;
using Gast.Unity.Shared.Observables;
using R3;
using DisposableBag = Gast.Core.Observables.DisposableBag;
using Random = UnityEngine.Random;

namespace Cryst.Features.Characters.Footsteps
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
                .Where(symbol => symbol == settings.FootstepEvent)
                .Subscribe(_ => PlayFootstep(audio, settings))
                .AddTo(disposableBag);
        }

        public void Dispose()
        {
            disposableBag.Dispose();
        }

        void PlayFootstep(CharacterAudio audio, CharacterFootstepSettings settings)
        {
            var audioSource = audio.FootStepAudioSource;

            // Select random footstep clip
            var clip = settings.FootstepClips[Random.Range(0, settings.FootstepClips.Length)];

            // Apply volume and pitch variation for natural sound
            audioSource.volume = settings.FootstepVolume +
                                 Random.Range(-settings.VolumeVariance, settings.VolumeVariance);
            audioSource.pitch = 1.0f + Random.Range(-settings.PitchVariance, settings.PitchVariance);

            audioSource.PlayOneShot(clip);
        }
    }
}