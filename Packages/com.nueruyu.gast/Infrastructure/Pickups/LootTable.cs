using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gast.Domain.Economy;
using Gast.Domain.Loot;
using Gast.Features.Economy;

namespace Gast.Infrastructure.Pickups
{
    [CreateAssetMenu(fileName = "NewLootTable", menuName = "Gast/Loot/Loot Table")]
    public class LootTable : ScriptableObject, ILootTable
    {
        [SerializeField]
        List<LootEntryData> entries = new();

        public IReadOnlyList<ILootTableEntry> Entries => entries;

        [Serializable]
        class LootEntryData : ILootTableEntry
        {
            [SerializeField]
            ItemReference itemReference;

            [SerializeField]
            [Range(0f, 1f)]
            float dropRate = 0.5f;

            [SerializeField]
            [Min(1)]
            int minQuantity = 1;

            [SerializeField]
            [Min(1)]
            int maxQuantity = 1;

            public ItemId ItemId => itemReference.Id;
            public float DropRate => dropRate;
            public int MinQuantity => minQuantity;
            public int MaxQuantity => maxQuantity;
        }
    }
}