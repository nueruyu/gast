using Gast.Core.Events;
using Gast.Domain.Loot;
using UnityEngine;

namespace Gast.Domain.Characters
{
    public readonly struct LootSpawnEvent : IDomainEvent
    {
        public ILootTable LootTable { get; }
        public Vector3 Position { get; }

        public LootSpawnEvent(ILootTable lootTable, Vector3 position)
        {
            LootTable = lootTable;
            Position = position;
        }
    }
}