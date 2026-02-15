using Cryst.Modules.CharacterActions;
using Gast.Features.Characters;

namespace Cryst.Infrastructure.Characters
{
    public class CharacterContextInitializer : ICharacterContextInitializer
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