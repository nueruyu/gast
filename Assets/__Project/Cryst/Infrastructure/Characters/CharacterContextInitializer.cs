using Cryst.Modules.CharacterActions;
using Gast.Features.Characters;
using Gast.Features.Combat;

namespace Cryst.Infrastructure.Characters
{
    public class CharacterContextInitializer : ICharacterContextInitializer
    {
        readonly IHitAreaFactory hitAreaFactory;

        public CharacterContextInitializer(IHitAreaFactory hitAreaFactory)
        {
            this.hitAreaFactory = hitAreaFactory;
        }

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
            context.Register(hitAreaFactory);
        }
    }
}