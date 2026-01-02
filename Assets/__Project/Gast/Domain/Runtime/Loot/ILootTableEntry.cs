using Gast.Domain.Economy;

namespace Gast.Domain.Loot
{
    public interface ILootTableEntry
    {
        ItemId ItemId { get; }
        float DropRate { get; }
        int MinQuantity { get; }
        int MaxQuantity { get; }
    }
}