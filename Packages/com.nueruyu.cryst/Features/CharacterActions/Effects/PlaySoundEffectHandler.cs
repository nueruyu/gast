using Gast.Unity.Features.Characters;

namespace Cryst.Features.CharacterActions.Effects
{
    /// <summary>
    /// Handles <see cref="PlaySoundEffect"/> by playing a one-shot clip
    /// via the character's <see cref="CharacterAudio"/>.
    /// </summary>
    public class PlaySoundEffectHandler : ICharacterActionEffectHandler<PlaySoundEffect>
    {
        readonly CharacterAudio audio;

        public PlaySoundEffectHandler(CharacterAudio audio)
        {
            this.audio = audio;
        }

        public void Handle(PlaySoundEffect effect)
        {
            if (audio == null || effect.Sfx == null) return;

            audio.OneShotAudioSource.PlayOneShot(effect.Sfx, effect.Volume);
        }
    }
}
