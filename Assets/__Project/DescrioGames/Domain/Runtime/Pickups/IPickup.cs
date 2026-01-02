using DescrioGames.Core.Observables;
using DescrioGames.Domain.Economy;
using UnityEngine;

namespace DescrioGames.Domain.Pickups
{
    public interface IPickup
    {
        PickupId Id { get; }
        int Quantity { get; }
        Vector3 Position { get; }
        ISignal<IPickup> Destroyed { get; }

        void Eject();
    }
}