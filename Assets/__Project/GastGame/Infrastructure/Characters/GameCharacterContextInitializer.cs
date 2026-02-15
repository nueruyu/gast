using Gast.Features.Characters;
using GastGame.Features.Characters;

namespace GastGame.Infrastructure.Characters
{
    public class GameCharacterContextInitializer : ICharacterContextInitializer
    {
        public void Initialize(CharacterContext context)
        {
            var animator = context.Body.GetComponentInChildren<CharacterAnimator>();
            var stateStore = new CharacterActionStateStore();
            var movement = new CharacterMovement(
                animator,
                context.Body,
                context.TypeDefinition);

            context.Register(animator);
            context.Register(stateStore);
            context.Register(movement);
        }
    }
}