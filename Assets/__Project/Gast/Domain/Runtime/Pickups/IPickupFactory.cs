using DescrioGames.Domain.Economy;
using UnityEngine;

namespace DescrioGames.Domain.Pickups
{
    public interface IPickupFactory
    {
        IPickup Create(ItemId itemId, int quantity, Vector3 position);
    }
}