using System.Collections.Generic;

namespace Gast.Domain.Pickups
{
    public interface IPickupRepository
    {
        void Register(IPickup pickup);

        void Unregister(PickupId id);

        IPickup Find(PickupId id);

        IReadOnlyList<IPickup> GetAll();
    }
}