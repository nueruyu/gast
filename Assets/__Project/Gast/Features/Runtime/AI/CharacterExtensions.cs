using Gast.Domain.Characters;
using Gast.Domain.Stats;

namespace Gast.Features.AI
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