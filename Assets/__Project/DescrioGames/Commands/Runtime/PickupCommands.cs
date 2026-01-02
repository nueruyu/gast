using DescrioGames.Core.Commands;
using DescrioGames.Domain.Characters;
using DescrioGames.Domain.Economy;
using DescrioGames.Domain.Loot;
using DescrioGames.Domain.Pickups;
using System;
using UnityEngine;

namespace DescrioGames.Commands
{
    [Serializable]
    public readonly struct SpawnGatheringItemCommand : ICommand<IPickup>
    {
        public ItemId ItemId { get; }
        public int Quantity { get; }
        public Vector3 Position { get; }

        public SpawnGatheringItemCommand(ItemId itemId, int quantity, Vector3 position)
        {
            ItemId = itemId;
            Quantity = quantity;
            Position = position;
        }
    }

    [Serializable]
    public readonly struct SpawnLootCommand : ICommand
    {
        public CharacterTypeId CharacterTypeId { get; }
        public Vector3 Position { get; }

        public SpawnLootCommand(CharacterTypeId characterTypeId, Vector3 position)
        {
            CharacterTypeId = characterTypeId;
            Position = position;
        }
    }
}