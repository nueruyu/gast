using Gast.Features.Characters;

namespace GastGame.Features.Characters
{
    public record CharacterActionContext(
        CharacterContext CharacterContext,
        CharacterAnimator CharacterAnimator,
        CharacterActionStateStore StateStore);
}
