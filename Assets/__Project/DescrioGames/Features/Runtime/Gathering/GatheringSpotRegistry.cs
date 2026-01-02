using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DescrioGames.Features.Gathering
{
    public class GatheringSpotRegistry : MonoBehaviour
    {
        [Header("Gathering")]
        [SerializeField]
        List<GatheringSpot> gatheringSpots = new();

        public IEnumerable<GatheringSpot> GetGatheringSpots()
        {
            return gatheringSpots
                .Where(x => x && x.isActiveAndEnabled);
        }
    }
}