using System.Collections.Generic;
using System.Linq;
using Gast.Domain.Pickups;

namespace Gast.Unity.Infrastructure.Pickups
{
    public class PickupRepository : IPickupRepository
    {
        readonly Dictionary<PickupId, IPickup> pickups = new();

        public void Register(IPickup pickup)
        {
            if (pickup == null)
                throw new System.ArgumentNullException(nameof(pickup));

            pickups[pickup.Id] = pickup;
        }

        public void Unregister(PickupId id)
        {
            pickups.Remove(id);
        }

        public IPickup Find(PickupId id)
        {
            pickups.TryGetValue(id, out var pickup);
            return pickup;
        }

        public IReadOnlyList<IPickup> GetAll()
        {
            return pickups.Values.ToList();
        }
    }
}