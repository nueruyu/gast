using Gast.Unity.Features.Characters;

namespace Cryst.Features.CharacterActions.Effects
{
    /// <summary>
    /// Handles <see cref="PlaySoundEffect"/> by playing a one-shot clip
    /// via the character's <see cref="CharacterAudio"/>.
    /// </summary>
    public class PlaySoundEffectHandler : ICharacterActionEffectHandler<PlaySoundEffect>
    {
        public void Handle(PlaySoundEffect effect, CharacterContext context)
        {
            var audio = context.Resolve<CharacterAudio>();
            if (audio == null || effect.Sfx == null) return;

            audio.AudioSource.PlayOneShot(effect.Sfx, effect.Volume);
        }
    }
}
