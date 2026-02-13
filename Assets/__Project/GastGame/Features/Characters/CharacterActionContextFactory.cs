using Gast.Features.Characters;

namespace GastGame.Features.Characters
{
    public static class CharacterActionContextFactory
    {
        public static CharacterActionContext Create(CharacterContext context)
        {
            var animator = context.Resolve<CharacterAnimator>();
            var stateStore = context.Resolve<CharacterActionStateStore>();
            return new CharacterActionContext(context, animator, stateStore);
        }
    }
}
