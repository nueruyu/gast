using System.Collections.Generic;

namespace DescrioGames.Domain.Loot
{
    public interface ILootTable
    {
        IReadOnlyList<ILootTableEntry> Entries { get; }
    }
}