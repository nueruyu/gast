using System.Collections.Generic;

namespace DescrioGames.Domain.Pickups
{
    public interface IPickupRepository
    {
        void Register(IPickup pickup);

        void Unregister(PickupId id);

        IPickup Find(PickupId id);

        IReadOnlyList<IPickup> GetAll();
    }
}