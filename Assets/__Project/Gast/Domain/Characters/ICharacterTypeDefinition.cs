using Gast.Domain.Loot;

namespace Gast.Domain.Characters
{
    public interface ICharacterTypeDefinition
    {
        CharacterTypeId TypeId { get; }
        string DisplayName { get; }
        float WalkSpeed { get; }
        float SprintSpeed { get; }

        ILootTable LootTable { get; }
    }
}