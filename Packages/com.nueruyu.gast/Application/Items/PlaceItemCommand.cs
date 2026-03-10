using System;
using Gast.Core.Commands;
using Gast.Domain.Characters;
using Gast.Domain.Economy;
using UnityEngine;

namespace Gast.Application.Items
{
    [Serializable]
    public readonly struct PlaceItemCommand : ICommand<bool>
    {
        public CharacterId PlacerId { get; }
        public ItemId ItemId { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public PlaceItemCommand(CharacterId placerId, ItemId itemId, Vector3 position, Quaternion rotation)
        {
            PlacerId = placerId;
            ItemId = itemId;
            Position = position;
            Rotation = rotation;
        }
    }
}
