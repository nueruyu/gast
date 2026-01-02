using Gast.Domain.Economy;
using UnityEngine;

namespace Gast.Domain.Pickups
{
    public interface IPickupFactory
    {
        IPickup Create(ItemId itemId, int quantity, Vector3 position);
    }
}