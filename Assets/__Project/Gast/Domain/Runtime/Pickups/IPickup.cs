using Gast.Core.Observables;
using Gast.Domain.Economy;
using UnityEngine;

namespace Gast.Domain.Pickups
{
    public interface IPickup
    {
        PickupId Id { get; }
        ItemId ItemId { get; }
        int Quantity { get; }
        Vector3 Position { get; }
        ISignal<IPickup> Destroyed { get; }

        void Eject();
    }
}