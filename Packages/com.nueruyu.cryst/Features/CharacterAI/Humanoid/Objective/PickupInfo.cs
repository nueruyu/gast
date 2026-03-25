using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public readonly struct PickupInfo
    {
        public PickupId Id { get; }
        public ItemId ItemId { get; }
        public Vector3 Position { get; }

        public PickupInfo(PickupId id, ItemId itemId, Vector3 position)
        {
            Id = id;
            ItemId = itemId;
            Position = position;
        }
    }
}
