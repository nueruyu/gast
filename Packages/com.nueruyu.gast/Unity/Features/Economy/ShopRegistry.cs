using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Gast.Features.Economy
{
    public class ShopRegistry : MonoBehaviour
    {
        [SerializeField]
        Shop[] shops = { };

        public Shop[] GetShops()
        {
            return shops
                .Where(x => x != null && x.isActiveAndEnabled)
                .ToArray();
        }
    }
}