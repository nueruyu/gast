using DescrioGames.Features.Characters;

namespace DescrioGames.Features.Combat
{
    public interface ICombatMethodFactory
    {
        ICombatMethod CreateMethod(CombatMethodSettings methodSettings);
    }
}