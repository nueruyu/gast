using Gast.Domain.Characters;
using Gast.Domain.Stats;

namespace GastGame.AI
{
    static class CharacterExtensions
    {
        public static bool IsAlive(this ICharacter character)
        {
            return character.Status.TryGetStatValue(StatId.FromString("Health"), out var health) &&
                health > 0;
        }
    }
}