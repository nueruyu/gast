using DescrioGames.Domain.AI;
using DescrioGames.Domain.Characters;
using DescrioGames.Domain.Sensors;

namespace DescrioGames.Features.Characters
{
    public record CharacterContext(
        CharacterId Id,
        CharacterTypeId TypeId,
        Faction Faction,
        CharacterBody Body,
        CharacterAnimator Animator,
        CharacterAnimationReceiver AnimationReceiver,
        CharacterAudio Audio,
        IVisionSensor VisionSensor,
        INavigationProvider NavigationProvider);
}