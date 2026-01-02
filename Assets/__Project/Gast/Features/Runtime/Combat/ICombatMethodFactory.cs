using Gast.Features.Characters;

namespace Gast.Features.Combat
{
    public interface ICombatMethodFactory
    {
        ICombatMethod CreateMethod(CombatMethodSettings methodSettings);
    }
}