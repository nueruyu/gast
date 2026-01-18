using Gast.Domain.AI;
using Gast.Domain.Characters;
using Gast.Domain.Interactions;
using Gast.Domain.Sensors;

namespace Gast.Features.Characters
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
        IInteractionSensor InteractionSensor,
        INavigationProvider NavigationProvider);
}