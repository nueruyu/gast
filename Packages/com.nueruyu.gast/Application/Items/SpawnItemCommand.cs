using Gast.Core.Commands;
using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using System;
using UnityEngine;

namespace Gast.Application.Items
{
    [Serializable]
    public readonly struct SpawnItemCommand : ICommand<IPickup>
    {
        public ItemId ItemId { get; }
        public int Quantity { get; }
        public Vector3 Position { get; }

        public SpawnItemCommand(ItemId itemId, int quantity, Vector3 position)
        {
            ItemId = itemId;
            Quantity = quantity;
            Position = position;
        }
    }
}
