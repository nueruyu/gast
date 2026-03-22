using Gast.Domain.Economy;
using Gast.Domain.Pickups;
using UnityEngine;

namespace Cryst.Features.CharacterAI.Humanoid.Objective
{
    public interface IPickupInfo
    {
        PickupId Id { get; }
        ItemId ItemId { get; }
        Vector3 Position { get; }
    }
}
