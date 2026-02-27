using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gast.Features.SpawnSites
{
    public class SpawnSiteRegistry : MonoBehaviour
    {
        [SerializeField]
        SpawnSite[] spawnSites = { };

        public IEnumerable<SpawnSite> GetSpawnSites()
        {
            return spawnSites
                .Where(x => x.isActiveAndEnabled);
        }
    }
}