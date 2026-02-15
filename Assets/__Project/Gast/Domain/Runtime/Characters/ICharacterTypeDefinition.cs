using Gast.Domain.Loot;
using Gast.Domain.Stats;

namespace Gast.Domain.Characters
{
    public interface ICharacterTypeDefinition
    {
        CharacterTypeId TypeId { get; }
        string DisplayName { get; }
        float WalkSpeed { get; }
        float SprintSpeed { get; }

        IStatSchema StatSchema { get; }

        ILootTable LootTable { get; }
    }
}
