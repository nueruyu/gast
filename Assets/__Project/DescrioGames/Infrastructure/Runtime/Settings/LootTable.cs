using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DescrioGames.Domain.Economy;
using DescrioGames.Domain.Loot;
using DescrioGames.Features.Economy;

namespace DescrioGames.Infrastructure.Settings
{
    [CreateAssetMenu(fileName = "NewLootTable", menuName = "DescrioGames/Loot/Loot Table")]
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