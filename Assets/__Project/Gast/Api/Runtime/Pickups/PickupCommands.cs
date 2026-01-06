using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using System;
using UnityEngine;

namespace Gast.Api.Pickups
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