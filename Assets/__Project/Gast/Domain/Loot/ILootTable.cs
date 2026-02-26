using System.Collections.Generic;

namespace Gast.Domain.Loot
{
    public interface ILootTable
    {
        IReadOnlyList<ILootTableEntry> Entries { get; }
    }
}