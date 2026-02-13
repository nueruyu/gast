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

            context.Register(animator);
            context.Register(stateStore);
        }
    }
}
