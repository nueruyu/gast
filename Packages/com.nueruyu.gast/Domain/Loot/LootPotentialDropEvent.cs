using Gast.Core.Events;
using UnityEngine;

namespace Gast.Domain.Loot
{
    public readonly struct LootPotentialDropEvent : IDomainEvent
    {
        public ILootTable LootTable { get; }
        public Vector3 Position { get; }

        public LootPotentialDropEvent(ILootTable lootTable, Vector3 position)
        {
            LootTable = lootTable;
            Position = position;
        }
    }
}